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
    private IConnection _connection;
    private IChannel _channel;
    private readonly UpdateSurveyResponseUseCase _updateSurveyResponseUseCase;
    private readonly string _queueName;

    public QuestionnaireUpdateConsumer(IConnectionFactory connectionFactory, IConfiguration configuration, UpdateSurveyResponseUseCase updateSurveyResponseUseCase)
    {
        _connectionFactory = connectionFactory;
        _queueName = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE_NAME") ?? "questionnaire-update-status";
        _updateSurveyResponseUseCase = updateSurveyResponseUseCase;
    }

    public async Task ConsumerMessageAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ConfigureConnection();
            await ConfigureQueuesAsync();
            await ConfigureConsumer(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_channel.IsOpen || !_connection.IsOpen)
                {
                    Log.Warning("[WARNING][RabbitMQ]: Conexão ou canal fechados. Tentando reconectar...");
                    await Task.Delay(5000, cancellationToken);
                    await ConfigureConnection();
                    await ConfigureQueuesAsync();
                }
                await Task.Delay(1000, cancellationToken); 
            }
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
        Log.Information("config conection");
        if (_connection == null || !_connection.IsOpen)
        {
            Log.Information("config conection é null");
            _connection?.Dispose();
            _connection = await _connectionFactory.CreateConnectionAsync();
        }
        Log.Information("config chanel");

        if (_channel == null || !_channel.IsOpen)
        {
            Log.Information("config chanel é null");
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
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            await ProcessMessageAsync(ea, cancellationToken);
        };

        await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer);
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

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
        catch (CustomException ex)
        {
            Log.Error($"[ERROR]: {ex.Message}");
            await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
        }
        catch (Exception ex)
        {
            Log.Error($"[ERROR][INESPERADO]: {ex.Message}");
            await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
