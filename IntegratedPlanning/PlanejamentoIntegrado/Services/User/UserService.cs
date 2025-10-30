using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PlanejamentoIntegrado.Constants;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class UserService(
    IRepository<User> userRepository,
    IMapper mapper,
    IPasswordHasher<User> passwordHasher
) : IUserService
{
    public async Task<bool> Register(UserViewModel model)
    {
        var existingUser = await userRepository.Get(u => u.Login == model.Login);
        if (existingUser != null)
            return false;

        var user = mapper.Map<User>(model);

        user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

        await userRepository.Insert(user);
        await userRepository.SaveChanges();

        return true;
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
