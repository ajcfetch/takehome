using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

/// <summary>
/// Used to convert any time only json field into a TimeOnly object
/// </summary>
public class TimeOnlyJsonConverter : JsonConverter<TimeOnly> {
  // While the sample data is HH:mm, we should plan for alternate or inconsistent data types.
  private static readonly string[] AcceptedFormats = {
    "HH:mm",        // 24-hour format (16:00)
    "hh:mm tt",     // 12-hour format (4:00 PM)
    "HH:mm:ss",     // 24-hour format with seconds (16:00:00)
    "hh:mm:ss tt"   // 12-hour format with seconds (4:00:00 PM)
  };

  /// <summary>
  /// Reads a json value and attempts to convert it into a TimeOnly object
  /// </summary>
  /// <param name="reader"></param>
  /// <param name="typeToConvert"></param>
  /// <param name="options"></param>
  /// <returns>TimeOnly</returns>
  /// <exception cref="JsonException"></exception>
  public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    string fieldName = JsonReaderHelper.GetPropertyName(ref reader);

    if (reader.TokenType != JsonTokenType.String) {
      throw new JsonException($"Invalid time format in '{fieldName}': expected a string.");
    }

    string timeString = reader.GetString()?.Trim() ?? throw new JsonException($"'{fieldName}' value is missing.");

    if (TimeOnly.TryParseExact(timeString, AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly parsedTime)) {
      return parsedTime;
    }

    throw new JsonException($"Invalid time format in '{fieldName}': '{timeString}'. Expected formats: {string.Join(", ", AcceptedFormats)}");
  }

  /// <summary>
  /// Converts a DateOnly object back into a string for a json response, uses 24h HH:mm
  /// </summary>
  /// <param name="writer"></param>
  /// <param name="value"></param>
  /// <param name="options"></param>
  public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString("HH:mm"));
  }
}
