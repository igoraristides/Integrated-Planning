using Microsoft.EntityFrameworkCore;
using PlanejamentoIntegrado.Data;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Repositories;

public class RepositoryTests : BaseTest
{
    private IntegratedPlanningDbContext _context;
    private Repository<User> _repository;

    public RepositoryTests()
    {
        _context = CreateInMemoryContext();
        _repository = new Repository<User>(_context);
    }

    [Fact]
    public async Task Insert_AddsEntityToDatabase()
    {
        var user = CreateTestUser();

        await _repository.Insert(user);
        await _repository.SaveChanges();

        var savedUser = await _context.Set<User>().FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Name, savedUser.Name);
        Assert.Equal(user.Email, savedUser.Email);
    }

    [Fact]
    public async Task Get_ById_ReturnsCorrectEntity()
    {
        var user = CreateTestUser();
        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.Get(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task Get_ByFilter_ReturnsCorrectEntity()
    {
        var users = CreateTestUsers();
        _context.Set<User>().AddRange(users);
        await _context.SaveChangesAsync();

        var result = await _repository.Get(u => u.Login == "joao.silva");

        Assert.NotNull(result);
        Assert.Equal("joao.silva", result.Login);
        Assert.Equal("João", result.Name);
    }

    [Fact]
    public async Task Get_NonExistingEntity_ReturnsNull()
    {
        var result = await _repository.Get(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_WithoutFilter_ReturnsAllEntities()
    {
        var users = CreateTestUsers();
        _context.Set<User>().AddRange(users);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAll();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_WithFilter_ReturnsFilteredEntities()
    {
        var users = CreateTestUsers();
        _context.Set<User>().AddRange(users);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAll(u =>
            u.ProfileId == Enums.SharedEnums.UserProfile.Admin
        );

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("João", result[0].Name);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsCorrectPage()
    {
        var users = CreateTestUsers();
        _context.Set<User>().AddRange(users);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAll(pageNumber: 1, pageSize: 1);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task Update_ModifiesEntity()
    {
        var user = CreateTestUser();
        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();

        user.Name = "Nome Atualizado";
        await _repository.Update(user);
        await _repository.SaveChanges();

        var updatedUser = await _context.Set<User>().FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("Nome Atualizado", updatedUser.Name);
    }

    [Fact]
    public async Task Delete_ById_RemovesEntity()
    {
        var user = CreateTestUser();
        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();

        await _repository.Delete(user.Id);
        await _repository.SaveChanges();

        var deletedUser = await _context.Set<User>().FindAsync(user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Delete_ByEntity_RemovesEntity()
    {
        var user = CreateTestUser();
        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();

        await _repository.Delete(user);
        await _repository.SaveChanges();

        var deletedUser = await _context.Set<User>().FindAsync(user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Delete_ByFilter_RemovesMatchingEntities()
    {
        var users = CreateTestUsers();
        _context.Set<User>().AddRange(users);
        await _context.SaveChangesAsync();

        await _repository.Delete(u =>
            u.ProfileId == PlanejamentoIntegrado.Enums.SharedEnums.UserProfile.Admin
        );
        await _repository.SaveChanges();

        var remainingUsers = await _context.Set<User>().ToListAsync();
        Assert.Single(remainingUsers);
        Assert.Equal(
            PlanejamentoIntegrado.Enums.SharedEnums.UserProfile.Viewer,
            remainingUsers[0].ProfileId
        );
    }

    [Fact]
    public async Task Count_WithFilter_ReturnsCorrectCount()
    {
        var users = CreateTestUsers();
        _context.Set<User>().AddRange(users);
        await _context.SaveChangesAsync();

        var count = await _repository.Count(u =>
            u.ProfileId == PlanejamentoIntegrado.Enums.SharedEnums.UserProfile.Admin
        );

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetMax_ReturnsMaxValue()
    {
        var users = CreateTestUsers();
        _context.Set<User>().AddRange(users);
        await _context.SaveChangesAsync();

        var maxId = await _repository.GetMax(u => (object)u.Id);

        Assert.Equal(2, maxId);
    }

    [Fact]
    public async Task SaveChanges_ReturnsNumberOfChanges()
    {
        var users = CreateTestUsers();

        await _repository.Insert(users[0]);
        await _repository.Insert(users[1]);
        var changes = await _repository.SaveChanges();

        Assert.Equal(2, changes);
    }
}
