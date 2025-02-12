using ApiGetewayAppPesquisa.Application.UseCases.UpdateSurveyReponse;
using ApiGetewayAppPesquisa.Infrastructure.Contexts;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;
using ApiGetewayAppPesquisa.Infrastructure.Repositorys;
using ApiGetewayAppPesquisa.Infrastructure.UoW;
using Microservice.UpdateQuestionnaire.Consumers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;

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
        builder.Services.AddSingleton<ProtocolUpdateStatusUseCase>();
        builder.Services.AddSingleton<ProtocolStatusUpdateConsumer>();
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
        var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var rabbitMqPort = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672");
        var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "guest";
        var rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "guest";

        builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
        {
            HostName = rabbitMqHost,
            Port = rabbitMqPort,
            UserName = rabbitMqUser,
            Password = rabbitMqPassword,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        });
    }
}
