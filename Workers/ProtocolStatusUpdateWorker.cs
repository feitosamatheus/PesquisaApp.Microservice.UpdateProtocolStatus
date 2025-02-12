using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.UpdateQuestionnaire.Consumers;

namespace Microservice.UpdateQuestionnaire.Workers;

public class ProtocolStatusUpdateWorker : BackgroundService
{
    private readonly ProtocolStatusUpdateConsumer _questionnaireUpdateConsumer;

    public ProtocolStatusUpdateWorker(ProtocolStatusUpdateConsumer questionnaireUpdateConsumer)
        =>  _questionnaireUpdateConsumer = questionnaireUpdateConsumer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _questionnaireUpdateConsumer.ConsumerMessageAsync(stoppingToken);
    }
}
