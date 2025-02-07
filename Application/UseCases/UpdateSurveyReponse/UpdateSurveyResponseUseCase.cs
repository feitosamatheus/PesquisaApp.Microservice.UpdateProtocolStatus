using ApiGetewayAppPesquisa.Application.Dtos;
using ApiGetewayAppPesquisa.Application.Exceptions;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;

namespace ApiGetewayAppPesquisa.Application.UseCases.UpdateSurveyReponse;

public class UpdateSurveyResponseUseCase
{
    private readonly IServiceProvider _serviceProvider;

    public UpdateSurveyResponseUseCase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> ExecuteAsync(QuestionnaireDTO dto, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var surveyRepository = scope.ServiceProvider.GetRequiredService<ISurveyRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnityOfWork>();

        var surveyResponse = await surveyRepository.GetSurveyResponseDetailByProtocolAsync(dto.Protocol, cancellationToken);
        surveyResponse.UpdateStatus(dto.Status);
        surveyResponse.SurveyBaseLine.UpdateStatus(dto.Status);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

