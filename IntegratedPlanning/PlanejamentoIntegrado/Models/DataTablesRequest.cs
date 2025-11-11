using System.Collections.Generic;

namespace PlanejamentoIntegrado.Models;

public class DataTablesRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DataTablesSearch? Search { get; set; }
    public List<DataTablesOrder>? Order { get; set; }
    public string? SupplierCode { get; set; }
    public List<string>? ModelIds { get; set; }
    public string? PartId { get; set; } // Mudado de int? para string? (ProductCode)
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? CreationStartDate { get; set; }
    public string? CreationEndDate { get; set; }
}

public class DataTablesSearch
{
    public string? Value { get; set; }
    public bool Regex { get; set; }
}

public class DataTablesOrder
{
    public int Column { get; set; }
    public string Dir { get; set; } = "asc";
}
