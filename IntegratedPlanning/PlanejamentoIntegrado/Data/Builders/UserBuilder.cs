using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data;

public class UserBuilder : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("PI_USER_SEC");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("USER_ID");

        builder.Property(u => u.Name).HasColumnName("USER_NAME").HasMaxLength(70);

        builder.Property(u => u.Surname).HasColumnName("USER_SURNAME").HasMaxLength(70);

        builder.Property(u => u.Email).HasColumnName("USER_EMAIL").HasMaxLength(70);

        builder.Property(u => u.Login).HasColumnName("USER_LOGIN").HasMaxLength(40);

        builder.Property(u => u.PasswordHash).HasColumnName("USER_PASSWORD_HASH").HasMaxLength(255);

        builder.Property(u => u.PasswordChangedAt).HasColumnName("DT_ALTEROU_PASSWORD");

        builder.Property(u => u.ChangedPassword).HasColumnName("IND_ALTERA_PASSWORD");

        builder.Property(u => u.IsActive).HasColumnName("IND_ACTIVE");

        builder.Property(u => u.ProfileId).HasColumnName("USER_PROFILE_ID");

        builder.Property(u => u.CreatedAt).HasColumnName("CREATION_DATE").HasColumnType("DATE");
    }
}
