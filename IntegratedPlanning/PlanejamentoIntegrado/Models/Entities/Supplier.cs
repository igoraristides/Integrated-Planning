namespace PlanejamentoIntegrado.Models;

public class Supplier
{
    /// <summary>VENDOR_CODE - Código do fornecedor</summary>
    public string SupplierCode { get; set; } = string.Empty;

    /// <summary>ORG_ID - Identificador da organização</summary>
    public int OrganizationId { get; set; }

    /// <summary>VENDOR_NAME - Nome do fornecedor</summary>
    public string? SupplierName { get; set; }

    /// <summary>EMAIL_ADDRESS - Endereço de e-mail</summary>
    public string? Email { get; set; }

    /// <summary>STATE_REGISTRATION - Inscrição estadual</summary>
    public string? StateRegistration { get; set; }

    /// <summary>VAT_REGISTRATION_NUM - CNPJ do fornecedor</summary>
    public string? Cnpj { get; set; }

    /// <summary>ADDRESS_LINE1 - Endereço (logradouro)</summary>
    public string? Address { get; set; }

    /// <summary>ADDRESS_LINE2 - Número do endereço</summary>
    public string? Number { get; set; }

    /// <summary>ADDRESS_LINE3 - Bairro</summary>
    public string? Neighborhood { get; set; }

    /// <summary>POSTAL_CODE - Código postal (CEP)</summary>
    public string? ZipCode { get; set; }

    /// <summary>CITY - Cidade</summary>
    public string? City { get; set; }

    /// <summary>STATE - Estado</summary>
    public string? State { get; set; }

    /// <summary>COUNTRY - País</summary>
    public string? Country { get; set; }

    /// <summary>AREA_CODE - Código de área do telefone</summary>
    public string? AreaCode { get; set; }

    /// <summary>PHONE - Número de telefone</summary>
    public string? Phone { get; set; }

    /// <summary>CREATION_DATE - Data de criação do registro</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>LAST_UPDATE_DATE - Data da última atualização</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>END_DATE_ACTIVE - Data de inativação</summary>
    public DateTime? InactivatedAt { get; set; }
}
