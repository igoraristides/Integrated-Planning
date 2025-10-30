using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Data.Builders;

public class SupplierBuilder : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("VW_M2V_PI_FORNECEDORES");

        builder.HasKey(s => s.SupplierCode);
        builder.Property(s => s.SupplierCode).HasColumnName("COD_FORNECEDOR").HasMaxLength(40);
        builder.Property(s => s.CreatedAt).HasColumnName("DT_CRIACAO");
        builder.Property(s => s.UpdatedAt).HasColumnName("DT_ATUALIZACAO");
        builder.Property(s => s.InactivatedAt).HasColumnName("DT_INATIVACAO");

        builder.Property(s => s.OrganizationId).HasColumnName("ORG_ID");

        builder.Property(s => s.SupplierName).HasColumnName("NOME_FORNECEDOR").HasMaxLength(240);

        builder.Property(s => s.Email).HasColumnName("EMAIL").HasMaxLength(2000);

        builder
            .Property(s => s.StateRegistration)
            .HasColumnName("INSCR_ESTADUAL")
            .HasMaxLength(150);

        builder.Property(s => s.Cnpj).HasColumnName("CNPJ").HasMaxLength(150);

        builder.Property(s => s.Address).HasColumnName("ENDERECO").HasMaxLength(240);

        builder.Property(s => s.Number).HasColumnName("NUMERO").HasMaxLength(80);

        builder.Property(s => s.Neighborhood).HasColumnName("BAIRRO").HasMaxLength(240);

        builder.Property(s => s.ZipCode).HasColumnName("CEP").HasMaxLength(80);

        builder.Property(s => s.City).HasColumnName("CIDADE").HasMaxLength(60);

        builder.Property(s => s.State).HasColumnName("UF").HasMaxLength(60);

        builder.Property(s => s.Country).HasColumnName("PAIS").HasMaxLength(100);

        builder.Property(s => s.AreaCode).HasColumnName("DDD_FONE").HasMaxLength(10);

        builder.Property(s => s.Phone).HasColumnName("TELEFONE").HasMaxLength(15);
    }
}
