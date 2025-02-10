using ApiGetewayAppPesquisa.Application.Enums;
using ApiGetewayAppPesquisa.Application.Exceptions;
using ApiGetewayAppPesquisa.Application.Helpers;

namespace ApiGetewayAppPesquisa.Application.Models;

public class SurveyBaseLine
{
    public int Id { get; private set; }
    public int Line { get; private set; }
    public int SurveyBaseId { get; private set; }
    public int SurveySampleId { get; private set; }
    public EStatusLine? Status { get; private set; }

    public void UpdateStatus(string status)
    {
        var statusEnum = EnumValidator.GetEnumStatusLineValueByDisplayName(status);
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
