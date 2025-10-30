using Microsoft.EntityFrameworkCore;
using PlanejamentoIntegrado.Data;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Tests.Base;

public class BaseTest
{
    protected static IntegratedPlanningDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<IntegratedPlanningDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new IntegratedPlanningDbContext(options);
        return context;
    }

    protected static User CreateTestUser(int id = 1) =>
        new()
        {
            Id = id,
            Name = "João",
            Surname = "Silva",
            Email = "joao.silva@teste.com",
            Login = "joao.silva",
            PasswordHash = "hashedpassword123",
            IsActive = 1,
            ProfileId = Enums.SharedEnums.UserProfile.Admin,
            CreatedAt = DateTime.Now
        };

    protected static List<User> CreateTestUsers() =>
        new()
        {
            new User
            {
                Id = 1,
                Name = "João",
                Surname = "Silva",
                Email = "joao.silva@teste.com",
                Login = "joao.silva",
                PasswordHash = "hashedpassword123",
                IsActive = 1,
                ProfileId = Enums.SharedEnums.UserProfile.Admin,
                CreatedAt = DateTime.Now
            },
            new User
            {
                Id = 2,
                Name = "Maria",
                Surname = "Santos",
                Email = "maria.santos@teste.com",
                Login = "maria.santos",
                PasswordHash = "hashedpassword456",
                IsActive = 1,
                ProfileId = Enums.SharedEnums.UserProfile.Viewer,
                CreatedAt = DateTime.Now
            }
        };
}
