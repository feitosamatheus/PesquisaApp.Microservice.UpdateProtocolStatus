using ApiGetewayAppPesquisa.Application.UseCases.UpdateSurveyReponse;
using ApiGetewayAppPesquisa.Infrastructure.Contexts;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;
using ApiGetewayAppPesquisa.Infrastructure.Repositorys;
using ApiGetewayAppPesquisa.Infrastructure.UoW;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ApiGetewayAppPesquisa.Infrastructure.Configurations;

public static class ConfigurationApp
{
    public static void ConfigureLogger(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.Console()
            .WriteTo.File("logs/LOG_API_.txt", rollingInterval: RollingInterval.Month)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
    }

    public static void ConfigureInjectionDependency(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<UpdateSurveyResponseUseCase>();
        builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
        builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();
    }

    public static void ConfigureDatabase(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApiGatewayContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PgSqlConnection")));
    }

    public static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));
    }
}
