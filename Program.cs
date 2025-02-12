using Microservice.UpdateQuestionnaire;
using Microservice.UpdateQuestionnaire.Configurations;
using Microservice.UpdateQuestionnaire.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();

ConfigurationApp.ConfigureLogger(builder);
ConfigurationApp.ConfigureDatabase(builder);
ConfigurationApp.ConfigureInjectionDependency(builder);
ConfigurationApp.ConfigureRabbitMQ(builder);

builder.Services.AddHostedService<ProtocolStatusUpdateWorker>();

var host = builder.Build();
host.Run();
