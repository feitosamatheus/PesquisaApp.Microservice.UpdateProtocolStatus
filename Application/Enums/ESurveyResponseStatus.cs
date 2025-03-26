using System.ComponentModel.DataAnnotations;

namespace ApiGetewayAppPesquisa.Application.Enums;

public enum ESurveyResponseStatus
{
    [Display(Name = "Pendente")]
    Pending = 1,

    [Display(Name = "Em andamento")]
    InProgress = 2,

    [Display(Name = "valida")]
    Completed = 3,

    [Display(Name = "naoatende")]
    NoAnswer = 4,

    [Display(Name = "telefoneerrado")]
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
    Proposal = 10,

    [Display(Name = "caixapostal")]
    VoicemailOrOutOfService = 11,

    [Display(Name = "Ligação Muda")]
    SilentCall = 12,

    [Display(Name = "recusa")]
    Refused = 13,

    [Display(Name = "Proposta aceita")]
    ProposalAceppt = 14,

    [Display(Name = "Proposta recusada")]
    ProposalRefused = 15,
    
    [Display(Name = "inexistente")]
    Inexistente = 16,

    [Display(Name = "retorno")]
    retorno = 17,
    
    [Display(Name = "duplicada")]
    duplicada = 18,

    [Display(Name = "naoparticipa")]
    naoparticipa = 19,

    [Display(Name = "naoseencontra")]
    naoseencontra = 20
}
