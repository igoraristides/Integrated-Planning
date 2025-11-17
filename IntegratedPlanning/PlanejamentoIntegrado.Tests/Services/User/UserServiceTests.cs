using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using PlanejamentoIntegrado.Constants;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;
using PlanejamentoIntegrado.Services;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Services.User;

public class UserServiceTests : BaseTest
{
    private readonly Mock<IRepository<PlanejamentoIntegrado.Models.User>> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IPasswordHasher<PlanejamentoIntegrado.Models.User>> _mockPasswordHasher;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IRepository<PlanejamentoIntegrado.Models.User>>();
        _mockMapper = new Mock<IMapper>();
        _mockPasswordHasher = new Mock<IPasswordHasher<PlanejamentoIntegrado.Models.User>>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockPasswordHasher.Object
        );
    }

    [Fact]
    public async Task Register_NewUser_ReturnsTrue()
    {
        var userViewModel = new UserViewModel
        {
            Name = "João",
            Surname = "Silva",
            Email = "joao@teste.com",
            Login = "joao.silva",
            Password = "senha123",
        };

        var user = CreateTestUser();
        var hashedPassword = "hashedpassword123";

        _mockUserRepository
            .Setup(x =>
                x.Get(It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.User, bool>>>(), null)
            )
            .ReturnsAsync((PlanejamentoIntegrado.Models.User?)null);

        _mockMapper
            .Setup(x => x.Map<PlanejamentoIntegrado.Models.User>(userViewModel))
            .Returns(user);
        _mockPasswordHasher
            .Setup(x => x.HashPassword(user, userViewModel.Password))
            .Returns(hashedPassword);
        _mockUserRepository.Setup(x => x.Insert(user)).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.SaveChanges(default)).ReturnsAsync(1);

        var result = await _userService.Register(userViewModel);

        Assert.True(result);
        _mockUserRepository.Verify(x => x.Insert(user), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChanges(default), Times.Once);
    }

    [Fact]
    public async Task Register_ExistingUser_ReturnsFalse()
    {
        var userViewModel = new UserViewModel { Login = "joao.existente" };

        var existingUser = CreateTestUser();

        _mockUserRepository
            .Setup(x =>
                x.Get(It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.User, bool>>>(), null)
            )
            .ReturnsAsync(existingUser);

        var result = await _userService.Register(userViewModel);

        Assert.False(result);
        _mockUserRepository.Verify(
            x => x.Insert(It.IsAny<PlanejamentoIntegrado.Models.User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAll_ReturnsListOfUserViewModels()
    {
        var users = CreateTestUsers();
        var userViewModels = new List<UserViewModel>
        {
            new UserViewModel
            {
                Id = 1,
                Name = "João",
                Login = "joao.silva",
            },
            new UserViewModel
            {
                Id = 2,
                Name = "Maria",
                Login = "maria.santos",
            },
        };

        _mockUserRepository.Setup(x => x.GetAll(null, 0, 0, null)).ReturnsAsync(users);
        _mockMapper.Setup(x => x.Map<List<UserViewModel>>(users)).Returns(userViewModels);

        var result = await _userService.GetAll();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("João", result[0].Name);
        Assert.Equal("Maria", result[1].Name);
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUserViewModel()
    {
        var user = CreateTestUser();
        var userViewModel = new UserViewModel
        {
            Id = 1,
            Name = "João",
            Login = "joao.silva",
        };

        _mockUserRepository.Setup(x => x.Get(1)).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserViewModel>(user)).Returns(userViewModel);

        var result = await _userService.GetById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("João", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNull()
    {
        _mockUserRepository
            .Setup(x => x.Get(999))
            .ReturnsAsync((PlanejamentoIntegrado.Models.User?)null);

        var result = await _userService.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Edit_ValidUser_ReturnsSuccessWithNoError()
    {
        var userViewModel = new UserViewModel
        {
            Id = 1,
            Name = "João Editado",
            Login = "joao.editado",
            Password = "novaSenha123",
        };

        var users = new List<PlanejamentoIntegrado.Models.User> { CreateTestUser() };

        _mockUserRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.User, bool>>>(),
                    0,
                    0,
                    null
                )
            )
            .ReturnsAsync(users);

        _mockMapper.Setup(x => x.Map(userViewModel, users[0]));
        _mockPasswordHasher
            .Setup(x => x.HashPassword(users[0], userViewModel.Password))
            .Returns("hashedPassword");
        _mockUserRepository.Setup(x => x.Update(users[0])).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.SaveChanges(default)).ReturnsAsync(1);

        var result = await _userService.Edit(1, userViewModel);

        Assert.True(result.Item1);
        Assert.Null(result.Item2);
    }

    [Fact]
    public async Task Edit_NonExistingUser_ReturnsFailureWithError()
    {
        var userViewModel = new UserViewModel { Id = 999, Name = "Inexistente" };

        _mockUserRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.User, bool>>>(),
                    0,
                    0,
                    null
                )
            )
            .ReturnsAsync(new List<PlanejamentoIntegrado.Models.User>());

        var result = await _userService.Edit(999, userViewModel);

        Assert.False(result.Item1);
        Assert.Equal(MessageConstants.UserNotFound, result.Item2);
    }

    [Fact]
    public async Task Edit_LoginAlreadyTaken_ReturnsFailureWithError()
    {
        var userViewModel = new UserViewModel
        {
            Id = 1,
            Login = "maria.santos",
            Password = "senha123",
        };

        var users = CreateTestUsers();

        _mockUserRepository
            .Setup(x =>
                x.GetAll(
                    It.IsAny<Expression<Func<PlanejamentoIntegrado.Models.User, bool>>>(),
                    0,
                    0,
                    null
                )
            )
            .ReturnsAsync(users);

        var result = await _userService.Edit(1, userViewModel);

        Assert.False(result.Item1);
        Assert.Equal(MessageConstants.UserLoginAlreadyExists, result.Item2);
    }

    [Fact]
    public async Task Delete_ExistingUser_ReturnsTrue()
    {
        var user = CreateTestUser();

        _mockUserRepository.Setup(x => x.Get(1)).ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.Delete(user)).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.SaveChanges(default)).ReturnsAsync(1);

        var result = await _userService.Delete(1);

        Assert.True(result);
        _mockUserRepository.Verify(x => x.Delete(user), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChanges(default), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExistingUser_ReturnsFalse()
    {
        _mockUserRepository
            .Setup(x => x.Get(999))
            .ReturnsAsync((PlanejamentoIntegrado.Models.User?)null);

        var result = await _userService.Delete(999);

        Assert.False(result);
        _mockUserRepository.Verify(
            x => x.Delete(It.IsAny<PlanejamentoIntegrado.Models.User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ChangePassword_ValidUser_ReturnsSuccessWithNoError()
    {
        var user = CreateTestUser();
        var currentPassword = "senhaAtual123";
        var newPassword = "novaSenha123";
        var hashedNewPassword = "hashedNovaSenha123";

        _mockUserRepository.Setup(x => x.Get(1)).ReturnsAsync(user);
        _mockPasswordHasher
            .Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, currentPassword))
            .Returns(PasswordVerificationResult.Success);
        _mockPasswordHasher
            .Setup(x => x.HashPassword(user, newPassword))
            .Returns(hashedNewPassword);
        _mockUserRepository.Setup(x => x.Update(user)).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.SaveChanges(default)).ReturnsAsync(1);

        var result = await _userService.ChangePassword(1, currentPassword, newPassword);

        Assert.True(result.success);
        Assert.Null(result.error);
        Assert.Equal(hashedNewPassword, user.PasswordHash);
        Assert.Equal(1, user.ChangedPassword);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChanges(default), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_NonExistingUser_ReturnsFailureWithError()
    {
        _mockUserRepository
            .Setup(x => x.Get(999))
            .ReturnsAsync((PlanejamentoIntegrado.Models.User?)null);

        var result = await _userService.ChangePassword(999, "senhaAtual", "novaSenha");

        Assert.False(result.success);
        Assert.Equal(MessageConstants.UserNotFound, result.error);
        _mockUserRepository.Verify(
            x => x.Update(It.IsAny<PlanejamentoIntegrado.Models.User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ChangePassword_WrongCurrentPassword_ReturnsFailureWithError()
    {
        var user = CreateTestUser();
        var currentPassword = "senhaErrada";
        var newPassword = "novaSenha123";

        _mockUserRepository.Setup(x => x.Get(1)).ReturnsAsync(user);
        _mockPasswordHasher
            .Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, currentPassword))
            .Returns(PasswordVerificationResult.Failed);

        var result = await _userService.ChangePassword(1, currentPassword, newPassword);

        Assert.False(result.success);
        Assert.Equal(MessageConstants.CurrentPasswordError, result.error);
        _mockUserRepository.Verify(
            x => x.Update(It.IsAny<PlanejamentoIntegrado.Models.User>()),
            Times.Never
        );
    }
}
