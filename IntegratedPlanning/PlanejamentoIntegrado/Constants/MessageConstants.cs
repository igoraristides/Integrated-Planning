using System.Diagnostics.CodeAnalysis;

namespace PlanejamentoIntegrado.Constants;

[ExcludeFromCodeCoverage]
public static class MessageConstants
{
    public const string UserCreatedSuccess = "Usuário criado com sucesso.";
    public const string UserUpdatedSuccess = "Usuário editado com sucesso.";
    public const string UserDeletedSuccess = "Usuário excluído com sucesso.";
    public const string UserLoginAlreadyExists = "Login já está em uso.";
    public const string UserNotFound = "Usuário não encontrado.";
    public const string UnexpectedError =
        "Erro inesperado. Tente novamente ou contate o administrador.";
    public const string CurrentPasswordError = "Senha atual incorreta.";
    public const string PasswordChangedSuccess = "Senha alterada com sucesso.";
}
