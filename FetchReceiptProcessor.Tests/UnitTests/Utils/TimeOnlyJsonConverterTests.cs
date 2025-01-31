using System;
using System.Text.Json;
using Xunit;
using System.Globalization;

public class TimeOnlyJsonConverterTests {
  private readonly JsonSerializerOptions _options;

  public TimeOnlyJsonConverterTests() {
    _options = new JsonSerializerOptions {
      Converters = { new TimeOnlyJsonConverter() }
    };
  }

  [Theory]
  [InlineData("\"16:00\"", 16, 0)]
  [InlineData("\"04:00 PM\"", 16, 0)]
  [InlineData("\"16:00:00\"", 16, 0)]
  [InlineData("\"04:00:00 PM\"", 16, 0)]
  [InlineData("\"00:30\"", 0, 30)]
  public void Read_ValidTimeFormats_ShouldDeserializeCorrectly(string json, int expectedHour, int expectedMinute) {
    TimeOnly time = JsonSerializer.Deserialize<TimeOnly>(json, _options);

    Assert.Equal(new TimeOnly(expectedHour, expectedMinute), time);
  }

  [Theory]
  [InlineData("\"25:00\"")] 
  [InlineData("\"12:60\"")] 
  [InlineData("\"not-a-time\"")]
  [InlineData("\"13:00 AM\"")]
  public void Read_InvalidTimeFormats_ShouldThrowJsonException(string json) {
    var exception = Assert.Throws<JsonException>(() =>
      JsonSerializer.Deserialize<TimeOnly>(json, _options));

    Assert.Contains("Invalid time format", exception.Message);
  }

  [Fact]
  public void Write_ShouldSerializeTimeOnlyToString() {
    TimeOnly time = new TimeOnly(16, 0); // 4:00 PM in 24-hour format

    string json = JsonSerializer.Serialize(time, _options);

    Assert.Equal("\"16:00\"", json); 
  }

  [Fact]
  public void Write_ShouldSerializeMidnightCorrectly() {
    TimeOnly time = new TimeOnly(0, 0); // Midnight

    string json = JsonSerializer.Serialize(time, _options);

    Assert.Equal("\"00:00\"", json);
  }
}
