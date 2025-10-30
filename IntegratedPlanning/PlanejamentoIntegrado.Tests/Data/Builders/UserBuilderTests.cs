using Microsoft.EntityFrameworkCore;
using PlanejamentoIntegrado.Data;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Tests.Data.Builders;

public class UserBuilderTests
{
    private readonly UserBuilder _userBuilder;

    public UserBuilderTests()
    {
        _userBuilder = new UserBuilder();
    }

    [Fact]
    public void UserBuilder_ImplementsIEntityTypeConfiguration()
    {
        Assert.IsAssignableFrom<IEntityTypeConfiguration<User>>(_userBuilder);
    }

    [Fact]
    public void Configure_ConfiguresAllRequiredProperties()
    {
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();
        var entityTypeBuilder = modelBuilder.Entity<User>();

        var exception = Record.Exception(() => _userBuilder.Configure(entityTypeBuilder));
        Assert.Null(exception);
    }

    [Fact]
    public void UserBuilder_CanBeInstantiated()
    {
        Assert.NotNull(_userBuilder);
    }
}
