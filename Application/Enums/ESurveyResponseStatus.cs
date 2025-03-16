using System.ComponentModel.DataAnnotations;

namespace ApiGetewayAppPesquisa.Application.Enums;

public enum ESurveyResponseStatus
{
    [Display(Name = "Pendente")]
    Pending = 1,

    [Display(Name = "Em andamento")]
    InProgress = 2,

    [Display(Name = "Realizada")]
    Completed = 3,

    [Display(Name = "Não atende")]
    NoAnswer = 4,

    [Display(Name = "Número invalido")]
    InvalidNumber = 5,

    [Display(Name = "Interrompida")]
    Interrupted = 6,

    [Display(Name = "Insucesso")]
    Insucesso = 7,

    [Display(Name = "Em processamento")]
    InProcessing = 8,

    [Display(Name = "Agendamento")]
    Scheduling = 9,

    [Display(Name = "Proposta")]
    Proposal = 10
}
