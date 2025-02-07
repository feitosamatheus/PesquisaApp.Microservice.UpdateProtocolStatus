using ApiGetewayAppPesquisa.Application.Enums;
using ApiGetewayAppPesquisa.Application.Exceptions;
using ApiGetewayAppPesquisa.Application.Helpers;

namespace ApiGetewayAppPesquisa.Application.Models;

public sealed class SurveyResponseDetail
{
    public int Id { get; private set; }
    public int SurveyId { get; private set; }
    public int SurveyBaseLineId { get; private set; }
    public int SurveySampleId { get; private set; }
    public int SurveyUserId { get; private set; }
    public DateTime? DateResponseAnswer { get; private set; }
    public DateTime? DateResponseAnswerLocal => DateResponseAnswer.HasValue ? DateResponseAnswer.Value.ToLocalTime() : null;
    public string Protocol { get; private set; }
    public ESurveyResponseStatus? Status { get; private set; }

    public SurveyBaseLine SurveyBaseLine { get; private set; }

    public void UpdateStatus(string status)
    {
        var statusEnum = EnumValidator.GetEnumSurveyResponseStatusValueByDisplayName(status);
        if (statusEnum != null)
        {
            Status = statusEnum;
        }
        else
        {
            throw new CustomException($"[Status inválido][{status}]. Informe um status válido.");
        }
    }
}
