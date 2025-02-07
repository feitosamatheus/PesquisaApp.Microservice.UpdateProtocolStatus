using System.Threading;
using ApiGetewayAppPesquisa.Application.Models;
using ApiGetewayAppPesquisa.Infrastructure.Contexts;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiGetewayAppPesquisa.Infrastructure.Repositorys;

public class SurveyRepository : ISurveyRepository
{
    private readonly ApiGatewayContext _apiGatewayContext;

    public SurveyRepository(ApiGatewayContext apiGatewayContext)
    {
        _apiGatewayContext = apiGatewayContext;
    }

    public async Task<SurveyResponseDetail> GetSurveyResponseDetailByProtocolAsync(string protocol, CancellationToken cancellationToken)
        => await _apiGatewayContext.surveyResponseDetail_tb
                                   .Include(srd => srd.SurveyBaseLine)
                                   .FirstOrDefaultAsync(srd => srd.Protocol == protocol, cancellationToken);

    public async Task UpdateSurveyBaseLineAsync(SurveyBaseLine surveyBaseLine, CancellationToken cancellationToken)
    {
        _apiGatewayContext.Entry(surveyBaseLine).State = EntityState.Modified;

        await _apiGatewayContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSurveyResponseDetailAsync(SurveyResponseDetail surveyResponse, CancellationToken cancellationToken)
    {
        _apiGatewayContext.Entry(surveyResponse).State = EntityState.Modified;

        await _apiGatewayContext.SaveChangesAsync(cancellationToken);
    }
}
