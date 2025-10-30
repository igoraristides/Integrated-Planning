using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Services;

public interface IAuthService
{
    Task<User?> Authenticate(string login, string password);
}
