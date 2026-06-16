using static PlanejamentoIntegrado.Enums.SharedEnums;

namespace PlanejamentoIntegrado.Models;

public class User
{
    /// <summary>ID - Identificador único do usuário</summary>
    public int Id { get; set; }

    /// <summary>NAME - Nome do usuário</summary>
    public string Name { get; set; }

    /// <summary>SURNAME - Sobrenome do usuário</summary>
    public string Surname { get; set; }

    /// <summary>EMAIL - Endereço de e-mail do usuário</summary>
    public string Email { get; set; }

    /// <summary>LOGIN - Login de acesso do usuário</summary>
    public string Login { get; set; }

    /// <summary>PASSWORD_HASH - Hash da senha do usuário</summary>
    public string PasswordHash { get; set; }

    /// <summary>PASSWORD_CHANGED_AT - Data da última alteração de senha</summary>
    public DateTime? PasswordChangedAt { get; set; }

    /// <summary>CHANGED_PASSWORD - Indica se o usuário já alterou a senha (0=Não, 1=Sim)</summary>
    public int? ChangedPassword { get; set; }

    /// <summary>IS_ACTIVE - Indica se o usuário está ativo (0=Inativo, 1=Ativo)</summary>
    public int? IsActive { get; set; }

    /// <summary>RECEIVES_EMAIL - Indica se o usuário recebe e-mail (0=Não, 1=Sim)</summary>
    public int? ReceivesEmail { get; set; }

    /// <summary>PROFILE_ID - Identificador do perfil de acesso do usuário</summary>
    public UserProfile? ProfileId { get; set; }

    /// <summary>CREATED_AT - Data de criação do registro</summary>
    public DateTime? CreatedAt { get; set; }
}
