using Microsoft.AspNetCore.Identity;
using Moq;
using PlanejamentoIntegrado.Repositories;
using PlanejamentoIntegrado.Services;
using PlanejamentoIntegrado.Tests.Base;
using System.Linq.Expressions;
using UserEntity = PlanejamentoIntegrado.Models.User;

namespace PlanejamentoIntegrado.Tests.Services.Auth;

public class AuthServiceTests : BaseTest
{
    private readonly Mock<IRepository<UserEntity>> _mockUserRepository;
    private readonly Mock<IPasswordHasher<UserEntity>> _mockPasswordHasher;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IRepository<UserEntity>>();
        _mockPasswordHasher = new Mock<IPasswordHasher<UserEntity>>();
        _authService = new AuthService(_mockUserRepository.Object, _mockPasswordHasher.Object);
    }

    [Fact]
    public async Task Authenticate_ValidCredentials_ReturnsUser()
    {
 
        var user = CreateTestUser();
        var login = "joao.silva";
        var password = "senha123";
        
        _mockUserRepository
            .Setup(x => x.Get(It.IsAny<Expression<Func<UserEntity, bool>>>(), null))
            .ReturnsAsync(user);
        
        _mockPasswordHasher
            .Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, password))
            .Returns(PasswordVerificationResult.Success);


        var result = await _authService.Authenticate(login, password);

       
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Login, result.Login);
    }

    [Fact]
    public async Task Authenticate_UserNotFound_ReturnsNull()
    {
 
        var login = "usuario.inexistente";
        var password = "senha123";
        
        _mockUserRepository
            .Setup(x => x.Get(It.IsAny<Expression<Func<UserEntity, bool>>>(), null))
            .ReturnsAsync((UserEntity?)null);


        var result = await _authService.Authenticate(login, password);

       
        Assert.Null(result);
    }

    [Fact]
    public async Task Authenticate_InvalidPassword_ReturnsNull()
    {
 
        var user = CreateTestUser();
        var login = "joao.silva";
        var password = "senhaerrada";
        
        _mockUserRepository
            .Setup(x => x.Get(It.IsAny<Expression<Func<UserEntity, bool>>>(), null))
            .ReturnsAsync(user);
        
        _mockPasswordHasher
            .Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, password))
            .Returns(PasswordVerificationResult.Failed);


        var result = await _authService.Authenticate(login, password);

       
        Assert.Null(result);
    }

    [Fact]
    public async Task Authenticate_InactiveUser_ReturnsNull()
    {
 
        var user = CreateTestUser();
        user.IsActive = 0;
        var login = "joao.silva";
        var password = "senha123";
        
        _mockUserRepository
            .Setup(x => x.Get(It.IsAny<Expression<Func<UserEntity, bool>>>(), null))
            .ReturnsAsync((UserEntity?)null); 


        var result = await _authService.Authenticate(login, password);

       
        Assert.Null(result);
    }


}
