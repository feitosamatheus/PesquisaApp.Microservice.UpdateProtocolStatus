using ApiGetewayAppPesquisa.Application.Dtos;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;
using Microservice.UpdateProtocolStatus.Application.Exceptions;

namespace ApiGetewayAppPesquisa.Application.UseCases.UpdateSurveyReponse;

public class ProtocolUpdateStatusUseCase
{
    private readonly IServiceProvider _serviceProvider;

    public ProtocolUpdateStatusUseCase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> ExecuteAsync(ProtocolDTO dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dto.Protocol) || string.IsNullOrEmpty(dto.Status))
            throw new ValueIsNullException("Os valores fornecidos estão nulos ou vazio.");

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

