namespace PlanejamentoIntegrado.Models;

public class StockDetailsViewModel
{
    public string? ProductCode { get; set; }
    public List<ChartDataPoint>? ChartData { get; set; }
    public List<MatrixRow>? QuantityMatrix { get; set; }
    public List<MatrixRow>? ValueMatrix { get; set; }
}

public class ChartDataPoint
{
    public string? Date { get; set; }
    public decimal Quantity { get; set; }
}

public class MatrixRow
{
    public string? OriginName { get; set; }
    public Dictionary<string, decimal?>? WeekValues { get; set; }
    public decimal? Total { get; set; }
    public decimal? CV { get; set; }
    public string? CLCV { get; set; }
    public decimal? DoH { get; set; }
}
