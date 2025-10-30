using System.ComponentModel.DataAnnotations;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Tests.Models.View;

public class UserViewModelTests
{
    [Fact]
    public void UserViewModel_Properties_CanBeSetAndRetrieved()
    {
        var userViewModel = new UserViewModel();

        userViewModel.Id = 1;
        userViewModel.Name = "João";
        userViewModel.Surname = "Silva";
        userViewModel.Email = "joao.silva@teste.com";
        userViewModel.Login = "joao.silva";
        userViewModel.Password = "senha123";
        userViewModel.ProfileId = Enums.SharedEnums.UserProfile.Admin;
        userViewModel.IsActive = true;

        Assert.Equal(1, userViewModel.Id);
        Assert.Equal("João", userViewModel.Name);
        Assert.Equal("Silva", userViewModel.Surname);
        Assert.Equal("joao.silva@teste.com", userViewModel.Email);
        Assert.Equal("joao.silva", userViewModel.Login);
        Assert.Equal("senha123", userViewModel.Password);
        Assert.Equal(Enums.SharedEnums.UserProfile.Admin, userViewModel.ProfileId);
        Assert.True(userViewModel.IsActive);
    }

    [Fact]
    public void UserViewModel_DefaultValues_AreCorrect()
    {
        var userViewModel = new UserViewModel();

        Assert.Null(userViewModel.Name);
        Assert.Null(userViewModel.Surname);
        Assert.Null(userViewModel.Email);
        Assert.Null(userViewModel.Login);
        Assert.Null(userViewModel.Password);
        Assert.Null(userViewModel.ProfileId);
        Assert.Null(userViewModel.IsActive);
    }

    [Fact]
    public void UserViewModel_Password_HasDataTypePasswordAttribute()
    {
        var property = typeof(UserViewModel).GetProperty(nameof(UserViewModel.Password));

        var dataTypeAttribute = property
            ?.GetCustomAttributes(typeof(DataTypeAttribute), false)
            .Cast<DataTypeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(dataTypeAttribute);
        Assert.Equal(DataType.Password, dataTypeAttribute.DataType);
    }

    [Fact]
    public void UserViewModel_ProfileId_CanBeSetToViewer()
    {
        var userViewModel = new UserViewModel();

        userViewModel.ProfileId = Enums.SharedEnums.UserProfile.Viewer;

        Assert.Equal(Enums.SharedEnums.UserProfile.Viewer, userViewModel.ProfileId);
    }

    [Fact]
    public void UserViewModel_IsActive_CanBeSetToFalse()
    {
        var userViewModel = new UserViewModel();

        userViewModel.IsActive = false;

        Assert.False(userViewModel.IsActive);
    }

    [Fact]
    public void UserViewModel_Password_CanBeNull()
    {
        var userViewModel = new UserViewModel();

        userViewModel.Password = null;

        Assert.Null(userViewModel.Password);
    }

    [Fact]
    public void UserViewModel_ProfileId_CanBeNull()
    {
        var userViewModel = new UserViewModel();

        userViewModel.ProfileId = null;

        Assert.Null(userViewModel.ProfileId);
    }
}
