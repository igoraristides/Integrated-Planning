namespace PlanejamentoIntegrado.Helpers;

public static class StringHelper
{
    public static string ExtractMiddlePartFromCode(string? code)
    {
        if (string.IsNullOrEmpty(code))
            return string.Empty;

        var parts = code.Split('-');

        if (parts.Length == 1)
            return code.Trim();

        if (parts.Length == 2)
            return parts[1].Trim();

        return parts[1].Trim();
    }
}
