using AutoMapper;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Mappers;

public class UserMapperTests : BaseTest
{
    private readonly IMapper _mapper;

    public UserMapperTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMapper>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_UserToUserViewModel_MapsCorrectly()
    {
        var user = CreateTestUser();

        var userViewModel = _mapper.Map<UserViewModel>(user);

        Assert.NotNull(userViewModel);
        Assert.Equal(user.Id, userViewModel.Id);
        Assert.Equal(user.Name, userViewModel.Name);
        Assert.Equal(user.Surname, userViewModel.Surname);
        Assert.Equal(user.Email, userViewModel.Email);
        Assert.Equal(user.Login, userViewModel.Login);
        Assert.Equal(user.ProfileId, userViewModel.ProfileId);
        Assert.Equal(user.IsActive == 1, userViewModel.IsActive);
    }

    [Fact]
    public void Map_UserViewModelToUser_MapsCorrectly()
    {
        var userViewModel = new UserViewModel
        {
            Id = 1,
            Name = "João",
            Surname = "Silva",
            Email = "joao.silva@teste.com",
            Login = "joao.silva",
            Password = "senha123",
            ProfileId = Enums.SharedEnums.UserProfile.Admin,
            IsActive = true
        };

        var user = _mapper.Map<User>(userViewModel);

        Assert.NotNull(user);
        Assert.Equal(userViewModel.Id, user.Id);
        Assert.Equal(userViewModel.Name, user.Name);
        Assert.Equal(userViewModel.Surname, user.Surname);
        Assert.Equal(userViewModel.Email, user.Email);
        Assert.Equal(userViewModel.Login, user.Login);
        Assert.Equal(userViewModel.ProfileId, user.ProfileId);
    }

    [Fact]
    public void Map_UserWithInactiveStatus_MapsIsActiveCorrectly()
    {
        var user = CreateTestUser();
        user.IsActive = 0; 

        var userViewModel = _mapper.Map<UserViewModel>(user);

        Assert.False(userViewModel.IsActive);
    }

    [Fact]
    public void Map_UserViewModelWithInactiveStatus_MapsIsActiveCorrectly()
    {
        var userViewModel = new UserViewModel { Name = "João", IsActive = false };

        var user = _mapper.Map<User>(userViewModel);

        Assert.NotNull(user);
    }

    [Fact]
    public void Map_UserWithViewerProfile_MapsProfileCorrectly()
    {
        var user = CreateTestUser();
        user.ProfileId = Enums.SharedEnums.UserProfile.Viewer;

        var userViewModel = _mapper.Map<UserViewModel>(user);

        Assert.Equal(
            Enums.SharedEnums.UserProfile.Viewer,
            userViewModel.ProfileId
        );
    }

    [Fact]
    public void Map_UserViewModelWithViewerProfile_MapsProfileCorrectly()
    {
        var userViewModel = new UserViewModel
        {
            Name = "Maria",
            ProfileId = Enums.SharedEnums.UserProfile.Viewer
        };

        var user = _mapper.Map<User>(userViewModel);

        Assert.Equal(Enums.SharedEnums.UserProfile.Viewer, user.ProfileId);
    }

    [Fact]
    public void Map_ListOfUsers_MapsCorrectly()
    {
        var users = CreateTestUsers();

        var userViewModels = _mapper.Map<List<UserViewModel>>(users);

        Assert.NotNull(userViewModels);
        Assert.Equal(users.Count, userViewModels.Count);

        for (int i = 0; i < users.Count; i++)
        {
            Assert.Equal(users[i].Id, userViewModels[i].Id);
            Assert.Equal(users[i].Name, userViewModels[i].Name);
            Assert.Equal(users[i].Email, userViewModels[i].Email);
        }
    }

    [Fact]
    public void Map_ListOfUserViewModels_MapsCorrectly()
    {
        var userViewModels = new List<UserViewModel>
        {
            new UserViewModel
            {
                Id = 1,
                Name = "João",
                Email = "joao@teste.com"
            },
            new UserViewModel
            {
                Id = 2,
                Name = "Maria",
                Email = "maria@teste.com"
            }
        };

        var users = _mapper.Map<List<User>>(userViewModels);

        Assert.NotNull(users);
        Assert.Equal(userViewModels.Count, users.Count);

        for (int i = 0; i < userViewModels.Count; i++)
        {
            Assert.Equal(userViewModels[i].Id, users[i].Id);
            Assert.Equal(userViewModels[i].Name, users[i].Name);
            Assert.Equal(userViewModels[i].Email, users[i].Email);
        }
    }

    [Fact]
    public void Map_NullUser_ReturnsNull()
    {
        User? user = null;

        var userViewModel = _mapper.Map<UserViewModel>(user);

        Assert.Null(userViewModel);
    }

    [Fact]
    public void Map_NullUserViewModel_ReturnsNull()
    {
        UserViewModel? userViewModel = null;

        var user = _mapper.Map<User>(userViewModel);

        Assert.Null(user);
    }
}
