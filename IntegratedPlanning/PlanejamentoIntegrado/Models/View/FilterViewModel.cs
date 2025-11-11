using System.Collections.Generic;

namespace PlanejamentoIntegrado.Models;

public class FilterViewModel
{
    public string? SupplierCode { get; set; }
    public int? PartId { get; set; }
    public List<string>? ModelIds { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CreationStartDate { get; set; }
    public DateTime? CreationEndDate { get; set; }
}
