using Microservice.UpdateQuestionnaire;
using Microservice.UpdateQuestionnaire.Configurations;
using Microservice.UpdateQuestionnaire.Workers;

var builder = Host.CreateApplicationBuilder(args);


ConfigurationApp.ConfigureLogger(builder);
ConfigurationApp.ConfigureDatabase(builder);
ConfigurationApp.ConfigureInjectionDependency(builder);
ConfigurationApp.ConfigureRabbitMQ(builder);

builder.Services.AddHostedService<QuestionnaireUpdateWorker>();

var host = builder.Build();
host.Run();
