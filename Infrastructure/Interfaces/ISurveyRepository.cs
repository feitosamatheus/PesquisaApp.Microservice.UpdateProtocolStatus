using ApiGetewayAppPesquisa.Application.Models;

namespace ApiGetewayAppPesquisa.Infrastructure.Interfaces;

public interface ISurveyRepository
{
    Task<SurveyResponseDetail> GetSurveyResponseDetailByProtocolAsync(string protocol, CancellationToken cancellationToken);
    Task UpdateSurveyResponseDetailAsync(SurveyResponseDetail surveyResponse, CancellationToken cancellationToken);
    Task UpdateSurveyBaseLineAsync(SurveyBaseLine surveyBaseLine, CancellationToken cancellationToken);
}
