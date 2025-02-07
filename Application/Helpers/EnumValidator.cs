using ApiGetewayAppPesquisa.Application.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ApiGetewayAppPesquisa.Application.Helpers;

public class EnumValidator
{
    public static EStatusLine? GetEnumStatusLineValueByDisplayName(string status)
    {
        foreach (var value in Enum.GetValues(typeof(EStatusLine)))
        {
            var memberInfo = typeof(EStatusLine).GetMember(value.ToString()).FirstOrDefault();
            var displayAttribute = memberInfo?.GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute?.Name?.Equals(status, StringComparison.OrdinalIgnoreCase) == true)
            {
                return (EStatusLine)value;
            }
        }

        return null; 
    }

    public static ESurveyResponseStatus? GetEnumSurveyResponseStatusValueByDisplayName(string status)
    {
        foreach (var value in Enum.GetValues(typeof(ESurveyResponseStatus)))
        {
            var memberInfo = typeof(ESurveyResponseStatus).GetMember(value.ToString()).FirstOrDefault();
            var displayAttribute = memberInfo?.GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute?.Name?.Equals(status, StringComparison.OrdinalIgnoreCase) == true)
            {
                return (ESurveyResponseStatus)value;
            }
        }

        return null;
    }
}
