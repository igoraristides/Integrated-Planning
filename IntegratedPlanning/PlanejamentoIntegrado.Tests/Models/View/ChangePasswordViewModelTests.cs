using System.ComponentModel.DataAnnotations;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Models.View;

public class ChangePasswordViewModelTests : BaseTest
{
    [Fact]
    public void ChangePasswordViewModel_ValidModel_IsValid()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "novaSenha123",
            ConfirmPassword = "novaSenha123",
        };

        var validationResults = ValidateModel(model);

        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChangePasswordViewModel_MissingCurrentPassword_IsInvalid()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "",
            NewPassword = "novaSenha123",
            ConfirmPassword = "novaSenha123",
        };

        var validationResults = ValidateModel(model);

        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("CurrentPassword"));
    }

    [Fact]
    public void ChangePasswordViewModel_MissingNewPassword_IsInvalid()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "",
            ConfirmPassword = "novaSenha123",
        };

        var validationResults = ValidateModel(model);

        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("NewPassword"));
    }

    [Fact]
    public void ChangePasswordViewModel_MissingConfirmPassword_IsInvalid()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "novaSenha123",
            ConfirmPassword = "",
        };

        var validationResults = ValidateModel(model);

        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("ConfirmPassword"));
    }

    [Fact]
    public void ChangePasswordViewModel_PasswordsDoNotMatch_IsInvalid()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "novaSenha123",
            ConfirmPassword = "senhasDiferentes123",
        };

        var validationResults = ValidateModel(model);

        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("ConfirmPassword"));
        Assert.Contains(validationResults, v => v.ErrorMessage.Contains("não coincidem"));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
