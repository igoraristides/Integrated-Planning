using PlanejamentoIntegrado.Helpers;

namespace PlanejamentoIntegrado.Tests.Helpers;

public class StringHelperTests
{
    [Fact]
    public void ExtractMiddlePartFromCode_WithThreeParts_ReturnsMiddlePart()
    {
        var code = "PART1-MIDDLE-PART3";

        var result = StringHelper.ExtractMiddlePartFromCode(code);

        Assert.Equal("MIDDLE", result);
    }

    [Fact]
    public void ExtractMiddlePartFromCode_WithTwoParts_ReturnsSecondPart()
    {
        var code = "PART1-MIDDLE";

        var result = StringHelper.ExtractMiddlePartFromCode(code);

        Assert.Equal("MIDDLE", result);
    }

    [Fact]
    public void ExtractMiddlePartFromCode_WithOnePart_ReturnsWholePart()
    {
        var code = "SINGLEPART";

        var result = StringHelper.ExtractMiddlePartFromCode(code);

        Assert.Equal("SINGLEPART", result);
    }

    [Fact]
    public void ExtractMiddlePartFromCode_WithNull_ReturnsEmptyString()
    {
        string? code = null;

        var result = StringHelper.ExtractMiddlePartFromCode(code);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ExtractMiddlePartFromCode_WithEmptyString_ReturnsEmptyString()
    {
        var code = "";

        var result = StringHelper.ExtractMiddlePartFromCode(code);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ExtractMiddlePartFromCode_WithWhitespace_TrimsResult()
    {
        var code = "PART1- MIDDLE -PART3";

        var result = StringHelper.ExtractMiddlePartFromCode(code);

        Assert.Equal("MIDDLE", result);
    }

    [Fact]
    public void ExtractMiddlePartFromCode_WithMultipleDashes_ReturnsSecondPart()
    {
        var code = "PART1-MIDDLE-PART3-PART4-PART5";

        var result = StringHelper.ExtractMiddlePartFromCode(code);

        Assert.Equal("MIDDLE", result);
    }
}
