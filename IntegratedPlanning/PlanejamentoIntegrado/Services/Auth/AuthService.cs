using Microsoft.AspNetCore.Identity;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class AuthService(
    IRepository<User> userRepository,
    IPasswordHasher<User> passwordHasher
) : IAuthService
{
    public async Task<User?> Authenticate(string login, string password)
    {
        var user = await userRepository.Get(u => u.Login == login && u.IsActive == 1);

        if (user == null)
            return null;

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result == PasswordVerificationResult.Success ? user : null;
    }
}
