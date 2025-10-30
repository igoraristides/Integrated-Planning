using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Services;

public interface IUserService
{
    Task<bool> Register(UserViewModel model);
    Task<List<UserViewModel>> GetAll();
    Task<UserViewModel?> GetById(int id);
    Task<(bool, string?)> Edit(int id, UserViewModel model);
    Task<bool> Delete(int id);
    Task<(bool success, string? error)> ChangePassword(
        int userId,
        string currentPassword,
        string newPassword
    );
}
