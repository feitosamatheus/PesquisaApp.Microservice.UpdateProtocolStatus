using System.ComponentModel.DataAnnotations;

namespace ApiGetewayAppPesquisa.Application.Enums;

public enum ESurveyResponseStatus
{
    [Display(Name = "Em andamento")]
    InProgress = 1,

    [Display(Name = "Concluída")]
    Completed = 2,

    [Display(Name = "Não atende")]
    NoAnswer = 3,

    [Display(Name = "Número invalido")]
    InvalidNumber = 4,
        
    [Display(Name = "Interrompida")]
    Interrupted = 5,
        
    [Display(Name = "Insucesso")]
    Insucesso = 6
}
