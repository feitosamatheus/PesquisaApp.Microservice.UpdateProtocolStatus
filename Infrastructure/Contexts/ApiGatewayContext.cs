using ApiGetewayAppPesquisa.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGetewayAppPesquisa.Infrastructure.Contexts;

public class ApiGatewayContext : DbContext
{
    public ApiGatewayContext(DbContextOptions<ApiGatewayContext> options) : base(options) { }

    public virtual DbSet<SurveyResponseDetail> surveyResponseDetail_tb  { get; set; }
    public virtual DbSet<SurveyBaseLine> surveyBaseLine_tb { get; set; }
}
