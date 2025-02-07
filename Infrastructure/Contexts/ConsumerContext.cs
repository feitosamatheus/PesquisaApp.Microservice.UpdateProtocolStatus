using ApiGetewayAppPesquisa.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGetewayAppPesquisa.Infrastructure.Contexts;

public class ConsumerContext : DbContext
{
    public ConsumerContext(DbContextOptions<ConsumerContext> options) : base(options) { }

    public virtual DbSet<SurveyResponseDetail> surveyResponseDetail_tb  { get; set; }
    public virtual DbSet<SurveyBaseLine> surveyBaseLine_tb { get; set; }
}
