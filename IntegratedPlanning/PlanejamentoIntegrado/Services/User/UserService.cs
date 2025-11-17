using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PlanejamentoIntegrado.Constants;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class UserService(
    IRepository<User> userRepository,
    IMapper mapper,
    IPasswordHasher<User> passwordHasher,
    ILogger<UserService> logger
) : IUserService
{
    public async Task<bool> Register(UserViewModel model)
    {
        try
        {
            logger.LogInformation("Iniciando registro de usuário. Login: {Login}", model.Login);

            var existingUser = await userRepository.Get(u => u.Login == model.Login);
            if (existingUser != null)
            {
                logger.LogWarning("Usuário já existe. Login: {Login}", model.Login);
                return false;
            }

            var user = mapper.Map<User>(model);
            logger.LogInformation("=== LOG DE DATA NO UserService ===");
            logger.LogInformation(
                "Usuário mapeado. ID: {Id}, Name: {Name}, Email: {Email}",
                user.Id,
                user.Name,
                user.Email
            );
            logger.LogInformation(
                "CreatedAt após mapeamento: {CreatedAt} (Tipo: {Type})",
                user.CreatedAt,
                user.CreatedAt?.GetType().FullName ?? "NULL"
            );
            if (user.CreatedAt.HasValue)
            {
                var dt = user.CreatedAt.Value;
                logger.LogInformation(
                    "CreatedAt detalhado - Year: {Year}, Month: {Month}, Day: {Day}, Hour: {Hour}, Minute: {Minute}, Second: {Second}",
                    dt.Year,
                    dt.Month,
                    dt.Day,
                    dt.Hour,
                    dt.Minute,
                    dt.Second
                );
                logger.LogInformation("CreatedAt ToString(): {ToString}", dt.ToString());
            }
            logger.LogInformation("=== FIM LOG DE DATA NO UserService ===");

            user.PasswordHash = passwordHasher.HashPassword(user, model.Password ?? string.Empty);
            logger.LogInformation(
                "Senha hash gerada. PasswordHash length: {Length}",
                user.PasswordHash?.Length ?? 0
            );

            await userRepository.Insert(user);
            logger.LogInformation(
                "Usuário inserido no repositório. ID: {Id}, CreatedAt: {CreatedAt}",
                user.Id,
                user.CreatedAt
            );

            await userRepository.SaveChanges();
            logger.LogInformation("SaveChanges executado com sucesso. ID final: {Id}", user.Id);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Erro ao registrar usuário. Login: {Login}, Erro: {Message}",
                model.Login,
                ex.Message
            );
            throw;
        }
    }

    //Feito dessa forma devido a não validação de unicidade do banco
    public async Task<(bool, string?)> Edit(int id, UserViewModel model)
    {
        var users = await userRepository.GetAll(u => u.Id == id || u.Login == model.Login);

        var userToEdit = users.SingleOrDefault(u => u.Id == id);

        if (userToEdit is null)
            return (false, MessageConstants.UserNotFound);

        var loginAlreadyTaken = users.Any(u => u.Id != id && u.Login == model.Login);

        if (loginAlreadyTaken)
            return (false, MessageConstants.UserLoginAlreadyExists);

        mapper.Map(model, userToEdit);

        if (!string.IsNullOrEmpty(model.Password))
            userToEdit.PasswordHash = passwordHasher.HashPassword(userToEdit, model.Password);

        await userRepository.Update(userToEdit);
        await userRepository.SaveChanges();

        return (true, null);
    }

    public async Task<List<UserViewModel>> GetAll()
    {
        var users = await userRepository.GetAll();
        return mapper.Map<List<UserViewModel>>(users);
    }

    public async Task<UserViewModel?> GetById(int id)
    {
        var user = await userRepository.Get(id);
        return user is null ? null : mapper.Map<UserViewModel>(user);
    }

    public async Task<bool> Delete(int id)
    {
        var user = await userRepository.Get(id);
        if (user == null)
            return false;

        await userRepository.Delete(user);
        await userRepository.SaveChanges();

        return true;
    }

    public async Task<(bool success, string? error)> ChangePassword(
        int userId,
        string currentPassword,
        string newPassword
    )
    {
        var user = await userRepository.Get(userId);
        if (user == null)
            return (false, MessageConstants.UserNotFound);

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);

        if (result != PasswordVerificationResult.Success)
            return (false, MessageConstants.CurrentPasswordError);

        user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
        user.PasswordChangedAt = DateTime.Now;
        user.ChangedPassword = 1;

        await userRepository.Update(user);
        await userRepository.SaveChanges();

        return (true, null);
    }
}
