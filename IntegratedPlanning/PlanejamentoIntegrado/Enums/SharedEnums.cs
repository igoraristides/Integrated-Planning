using System.Diagnostics.CodeAnalysis;

namespace PlanejamentoIntegrado.Enums;

[ExcludeFromCodeCoverage]
public class SharedEnums
{
    public enum UserProfile
    {
        Admin,
        Viewer,
    }
}

public enum VariationType
{
    Up,
    Down,
}
