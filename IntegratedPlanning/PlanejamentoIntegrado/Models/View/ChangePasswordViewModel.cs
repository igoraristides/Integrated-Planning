using System.ComponentModel.DataAnnotations;

namespace PlanejamentoIntegrado.Models;

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Senha Atual")]
    public string CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Nova Senha")]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmação da Nova Senha")]
    [Compare("NewPassword", ErrorMessage = "A nova senha e a confirmação não coincidem.")]
    public string ConfirmPassword { get; set; }
}
