using System;
using System.Text.Json;
using Xunit;
using System.Text.Json.Serialization;
using System.Globalization;

public class DecimalJsonConverterTests {
    private readonly JsonSerializerOptions _options;

    public DecimalJsonConverterTests() {
        _options = new JsonSerializerOptions {
            Converters = { new DecimalJsonConverter() }
        };
    }

    [Theory]
    [InlineData("123.45", 123.45)]
    [InlineData("\"123.45\"", 123.45)]
    [InlineData("\"123\"", 123)]
    [InlineData("123", 123)]
    [InlineData("\"0.00\"", 0.00)]
    public void Read_ValidDecimalFormats_ShouldDeserializeCorrectly(string json, decimal expectedDecimal) {
        decimal value = JsonSerializer.Deserialize<decimal>(json, _options);

        Assert.Equal(expectedDecimal, value);
    }

    [Theory]
    [InlineData("\"invalidDecimal\"")]
    [InlineData("\"123,45\"")]
    [InlineData("\"$123.45\"")]
    [InlineData("\"abc123\"")]
    public void Read_InvalidDecimalFormats_ShouldThrowJsonException(string json) {
        var exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<decimal>(json, _options));

        Assert.Contains("Invalid decimal format", exception.Message);
    }

    [Fact]
    public void Write_ShouldSerializeDecimalToStringWithTwoDecimalPlaces() {
        decimal value = 123.456m;

        string json = JsonSerializer.Serialize(value, _options);

        Assert.Equal("\"123.46\"", json);  // Rounds to two decimal places
    }

    [Fact]
    public void Write_ShouldSerializeZeroToStringWithTwoDecimalPlaces() {
        decimal value = 0m;

        string json = JsonSerializer.Serialize(value, _options);

        Assert.Equal("\"0.00\"", json); // Ensures two decimal placew
    }
}
