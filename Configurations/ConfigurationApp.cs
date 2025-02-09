using Microservice.UpdateQuestionnaire.Consumers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using UpdateQuestionnaire.Application.UseCases.UpdateSurveyReponse;
using UpdateQuestionnaire.Domain.Interfaces;
using UpdateQuestionnaire.Infrastructure.Contexts;
using UpdateQuestionnaire.Infrastructure.Repositorys;
using UpdateQuestionnaire.Infrastructure.UoW;

namespace Microservice.UpdateQuestionnaire.Configurations;

public static class ConfigurationApp
{
    public static void ConfigureLogger(HostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
    }

    public static void ConfigureInjectionDependency(HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<UpdateSurveyResponseUseCase>();
        builder.Services.AddSingleton<QuestionnaireUpdateConsumer>();

        builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
        builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();
    }

    public static void ConfigureDatabase(HostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ConsumerContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PgSqlConnection")));
    }

    public static void ConfigureRabbitMQ(HostApplicationBuilder builder)
    {
        var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
        builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
        {
            HostName = rabbitMQConfig["Host"],
            Port = int.Parse(rabbitMQConfig["Port"])
        });
    }
}
