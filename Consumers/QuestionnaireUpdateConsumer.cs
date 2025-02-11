using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ApiGetewayAppPesquisa.Application.Dtos;
using ApiGetewayAppPesquisa.Application.Exceptions;
using ApiGetewayAppPesquisa.Application.UseCases.UpdateSurveyReponse;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace Microservice.UpdateQuestionnaire.Consumers;

public class QuestionnaireUpdateConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly UpdateSurveyResponseUseCase _updateSurveyResponseUseCase;
    private readonly string _queueName;
    private readonly int _queueRetryMax;
    private IConnection _connection;
    private IChannel _channel;

    public QuestionnaireUpdateConsumer(IConnectionFactory connectionFactory, IConfiguration configuration, UpdateSurveyResponseUseCase updateSurveyResponseUseCase)
    {
        _connectionFactory = connectionFactory;
        _queueName = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE_NAME") ?? "questionnaire-update-status";
        _queueRetryMax = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_MAX_RETRY") ?? "3");
        _updateSurveyResponseUseCase = updateSurveyResponseUseCase;
    }

    public async Task ConsumerMessageAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ConfigureConnection();
            await ConfigureQueuesAsync();
            await ConfigureConsumer(cancellationToken);
        }
        catch (BrokerUnreachableException ex)
        {
            Log.Error(ex, "[ERROR][RabbitMQ]: RabbitMQ não está acessível. Tentando novamente em 5 segundos...");
            await Task.Delay(5000, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ERROR][INESPERADO]: Erro inesperado ao conectar ao RabbitMQ.");
            await Task.Delay(5000, cancellationToken);
        }
    }

    private async Task ConfigureConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            Log.Warning("Conexão com RabbitMQ será recriada.");
            _connection?.Dispose();
            _connection = await _connectionFactory.CreateConnectionAsync();
        }

        if (_channel == null || !_channel.IsOpen)
        {
            Log.Warning("Canal com RabbitMQ será recriado.");
            _channel?.Dispose();
            _channel = await _connection.CreateChannelAsync();
        }
    }

    private async Task ConfigureQueuesAsync()
    {
        try
        {
            await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", $"{_queueName}_dlq" }
            });

            await _channel.QueueDeclareAsync(queue: $"{_queueName}_dlq", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ERROR][RabbitMQ]: Erro ao configurar as filas.");
            throw;
        }
    }

    private async Task ConfigureConsumer(CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            await ProcessMessageAsync(ea, cancellationToken);
        };

    }

    public async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        try
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var questionnaireDTO = JsonConvert.DeserializeObject<QuestionnaireDTO>(message);
            if (questionnaireDTO != null)
            {
                await _updateSurveyResponseUseCase.ExecuteAsync(questionnaireDTO, cancellationToken);
            }
            else
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
        catch (CustomException ex)
        {
            Log.Error($"[ERROR]: {ex.Message}");
            await ProcessMessageFailAsync(ea, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error($"[ERROR][INESPERADO]: {ex.Message}");
            await ProcessMessageFailAsync(ea, cancellationToken);
        }
    }

    public async Task ProcessMessageFailAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        int retryCount = 0;
        if (ea.BasicProperties.Headers.TryGetValue("retry-count", out var retryHeader))
            retryCount = Convert.ToInt32(retryHeader);

        if (retryCount <= _queueRetryMax)
        {
            retryCount++;

            var properties = new BasicProperties
            {
                Persistent = true,
                Priority = 9,
                Expiration = "86400000",
                AppId = "api_gateway"
            };

            properties.Headers = new Dictionary<string, object>();
            properties.Headers["retry-count"] = retryCount;

            var publicationAddress = new PublicationAddress("", "", _queueName);
            await _channel.BasicPublishAsync(addr: publicationAddress, basicProperties: properties, body: ea.Body, cancellationToken);
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
        else
        {
            await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
