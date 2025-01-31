using System;
using System.Text.Json;
using Xunit;
using System.Text.Json.Serialization;
using System.Globalization;

public class DateOnlyJsonConverterTests {
  private readonly JsonSerializerOptions _options;

  public DateOnlyJsonConverterTests() {
    _options = new JsonSerializerOptions {
      Converters = { new DateOnlyJsonConverter() }
    };
  }

  [Theory]
  [InlineData("\"2025-01-01\"", "2025-01-01")]
  [InlineData("\"January 1, 2025\"", "2025-01-01")]
  [InlineData("\"Jan 1, 2025\"", "2025-01-01")]
  [InlineData("\"1 January 2025\"", "2025-01-01")]
  [InlineData("\"1 Jan 2025\"", "2025-01-01")]
  [InlineData("\"2025/01/01\"", "2025-01-01")]
  [InlineData("\"2025.01.01\"", "2025-01-01")]
  public void Read_ValidDates_ShouldDeserializeCorrectly(string json, string expectedDate) {
    DateOnly date = JsonSerializer.Deserialize<DateOnly>(json, _options);

    Assert.Equal(DateOnly.Parse(expectedDate), date);
  }

  [Theory]
  [InlineData("\"2025-01-32\"")]
  [InlineData("\"2025-13-01\"")]
  [InlineData("\"invalid-date\"")]
  public void Read_InvalidDates_ShouldThrowJsonException(string json) {
    var exception = Assert.Throws<JsonException>(() =>
      JsonSerializer.Deserialize<DateOnly>(json, _options));

    Assert.Contains("Invalid date format", exception.Message);
  }

  [Fact]
  public void Write_ShouldSerializeDateOnlyToString() {
    DateOnly date = DateOnly.Parse("2025-01-01");

    string json = JsonSerializer.Serialize(date, _options);

    Assert.Equal("\"2025-01-01\"", json);
  }
}
