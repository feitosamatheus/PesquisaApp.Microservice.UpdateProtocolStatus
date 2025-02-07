using ApiGetewayAppPesquisa.Application.Dtos;
using ApiGetewayAppPesquisa.Application.Exceptions;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;

namespace ApiGetewayAppPesquisa.Application.UseCases.UpdateSurveyReponse;

public class UpdateSurveyResponseUseCase
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUnityOfWork _unityOfWork;

    public UpdateSurveyResponseUseCase(ISurveyRepository surveyRepository, IUnityOfWork unityOfWork)
    {
        _surveyRepository = surveyRepository;
        _unityOfWork = unityOfWork;
    }

    public async Task<bool> ExecuteAsync(QuestionnaireDTO dto, CancellationToken cancellationToken)
    {

        var surveyResponse = await _surveyRepository.GetSurveyResponseDetailByProtocolAsync(dto.Protocol, cancellationToken);
        surveyResponse.UpdateStatus(dto.Status);
        surveyResponse.SurveyBaseLine.UpdateStatus(dto.Status);
        await _unityOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

