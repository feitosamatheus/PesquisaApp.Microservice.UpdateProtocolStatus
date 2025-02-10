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
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ConfigureConnection();
                await ConfigureQueuesAsync();

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    await ProcessMessageAsync(ea, cancellationToken);
                };

                await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer);

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
    }

    private async Task ConfigureConnection()
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    private async Task ConfigureQueuesAsync()
    {
        await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", $"{_queueName}_dlq" }
        });
        await _channel.QueueDeclareAsync(queue: $"{_queueName}_dlq", durable: true, exclusive: false, autoDelete: false, arguments: null);
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

            await Task.Delay(1000, cancellationToken);
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
