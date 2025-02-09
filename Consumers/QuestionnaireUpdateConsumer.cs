using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using UpdateQuestionnaire.Application.Dtos;
using UpdateQuestionnaire.Application.UseCases.UpdateSurveyReponse;
using UpdateQuestionnaire.Domain.Exceptions;

namespace Microservice.UpdateQuestionnaire.Consumers;

public class QuestionnaireUpdateConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly UpdateSurveyResponseUseCase _updateSurveyResponseUseCase;
    private readonly string _queueName;

    public QuestionnaireUpdateConsumer(IConnectionFactory connectionFactory, IConfiguration configuration, UpdateSurveyResponseUseCase updateSurveyResponseUseCase)
    {
        _connectionFactory = connectionFactory;
        _queueName = configuration.GetValue<string>("RabbitMQ:QueueName");
        _updateSurveyResponseUseCase = updateSurveyResponseUseCase;
    }

    public async Task ConsumerMessageAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueDeclareAsync(queue: $"{_queueName}_dlq", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea)   =>
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

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (CustomException ex)
                {
                    Log.Error($"[ERROR]: {ex.Message}");
                    await channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
                catch (Exception ex)
                {
                    Log.Error($"[ERROR][[INESPERADO]: ]: {ex.Message}");
                    await channel.BasicNackAsync(ea.DeliveryTag, false, false); 
                }
            };

            await channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer);
        }
        catch (Exception ex)
        {
            Log.Error($"[ERROR][RabbitMQ]: {ex.Message}");
        }
    }
}
