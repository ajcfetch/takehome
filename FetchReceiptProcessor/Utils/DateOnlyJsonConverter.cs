using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

/// <summary>
/// Used to convert any date only json field into a DateOnly object
/// </summary>
public class DateOnlyJsonConverter : JsonConverter<DateOnly> {
  // While the sample data is yyyy-MM-dd, we should plan for alternate or inconsistent data types.
  // There are many more we could target, but let's focus on just a few that are human-readable since
  // that is most likely to be what would be on a receipt.
  // ** We will not spend energy here getting hung up on us vs rest-of-the-world of mm/dd/yyyy or dd/mm/yyyy
  private static readonly string[] AcceptedFormats = {
    "yyyy-MM-dd",   // 2025-01-01 (preferred)
    "MMMM d, yyyy", // "January 1, 2025"
    "MMM d, yyyy",  // "Jan 1, 2025"
    "d MMMM yyyy",  // "1 January 2025"
    "d MMM yyyy",   // "1 Jan 2025"
    "yyyy/MM/dd",   // "2025/01/01"
    "yyyy.MM.dd"    // "2025.01.01"
  };

  /// <summary>
  /// Reads a json value and attempts to convert it into a DateOnly object
  /// </summary>
  /// <param name="reader"></param>
  /// <param name="typeToConvert"></param>
  /// <param name="options"></param>
  /// <returns>DateOnly</returns>
  /// <exception cref="JsonException"></exception>
  public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    string fieldName = JsonReaderHelper.GetPropertyName(ref reader);

    if (reader.TokenType != JsonTokenType.String) {
      throw new JsonException($"Invalid date format in '{fieldName}': expected a string.");
    }

    string dateString = reader.GetString()?.Trim() ?? throw new JsonException($"'{fieldName}' value is missing.");

    if (DateOnly.TryParseExact(dateString, AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDate)) {
      return parsedDate;
    }

    throw new JsonException($"Invalid date format in '{fieldName}': '{dateString}'. Expected formats: {string.Join(", ", AcceptedFormats)}");
  }

  /// <summary>
  /// Converts a DateOnly object back into a string for a json response, uses yyyy-MM-dd
  /// </summary>
  /// <param name="writer"></param>
  /// <param name="value"></param>
  /// <param name="options"></param>
  public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
  }
}
