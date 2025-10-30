using System.ComponentModel.DataAnnotations;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Tests.Models.View;

public class LoginViewModelTests
{
    [Fact]
    public void LoginViewModel_Properties_CanBeSetAndRetrieved()
    {
        var loginViewModel = new LoginViewModel();

        loginViewModel.Login = "usuario.teste";
        loginViewModel.Password = "senha123";

        Assert.Equal("usuario.teste", loginViewModel.Login);
        Assert.Equal("senha123", loginViewModel.Password);
    }

    [Fact]
    public void LoginViewModel_DefaultValues_AreCorrect()
    {
        var loginViewModel = new LoginViewModel();

        Assert.Null(loginViewModel.Login);
        Assert.Null(loginViewModel.Password);
    }

    [Fact]
    public void LoginViewModel_Login_HasRequiredAttribute()
    {
        var property = typeof(LoginViewModel).GetProperty(nameof(LoginViewModel.Login));

        var requiredAttribute = property
            ?.GetCustomAttributes(typeof(RequiredAttribute), false)
            .Cast<RequiredAttribute>()
            .FirstOrDefault();

        Assert.NotNull(requiredAttribute);
    }

    [Fact]
    public void LoginViewModel_Password_HasRequiredAttribute()
    {
        var property = typeof(LoginViewModel).GetProperty(nameof(LoginViewModel.Password));

        var requiredAttribute = property
            ?.GetCustomAttributes(typeof(RequiredAttribute), false)
            .Cast<RequiredAttribute>()
            .FirstOrDefault();

        Assert.NotNull(requiredAttribute);
    }

    [Fact]
    public void LoginViewModel_Password_HasDataTypePasswordAttribute()
    {
        var property = typeof(LoginViewModel).GetProperty(nameof(LoginViewModel.Password));

        var dataTypeAttribute = property
            ?.GetCustomAttributes(typeof(DataTypeAttribute), false)
            .Cast<DataTypeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(dataTypeAttribute);
        Assert.Equal(DataType.Password, dataTypeAttribute.DataType);
    }

    [Fact]
    public void LoginViewModel_ValidationAttributes_Count()
    {
        var loginProperty = typeof(LoginViewModel).GetProperty(nameof(LoginViewModel.Login));
        var passwordProperty = typeof(LoginViewModel).GetProperty(nameof(LoginViewModel.Password));

        var loginAttributes = loginProperty?.GetCustomAttributes(
            typeof(ValidationAttribute),
            false
        );
        var passwordAttributes = passwordProperty?.GetCustomAttributes(
            typeof(ValidationAttribute),
            false
        );

        Assert.NotNull(loginAttributes);
        Assert.Single(loginAttributes);

        Assert.NotNull(passwordAttributes);
        Assert.Equal(2, passwordAttributes.Length);
    }
}

