using System.ComponentModel.DataAnnotations;
using static PlanejamentoIntegrado.Enums.SharedEnums;

namespace PlanejamentoIntegrado.Models;

public class UserViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }
    public UserProfile? ProfileId { get; set; }
    public bool? IsActive { get; set; }
}