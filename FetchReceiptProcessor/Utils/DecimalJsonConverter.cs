using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;


/// <summary>
/// Used to convert any json data that is representing a price and coming in as a string or a number
/// </summary>
public class DecimalJsonConverter : JsonConverter<decimal> {

  /// <summary>
  /// Reads a json value and attempts to convert it into a decimal object
  /// </summary>
  /// <param name="reader"></param>
  /// <param name="typeToConvert"></param>
  /// <param name="options"></param>
  /// <returns>Decimal object</returns>
  /// <exception cref="JsonException"></exception>
  public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    string fieldName = JsonReaderHelper.GetPropertyName(ref reader);

    // Handle cases where the value is a number and not a string like in the example
    if (reader.TokenType == JsonTokenType.Number) {
      return reader.GetDecimal();
    }

    if (reader.TokenType != JsonTokenType.String) {
      throw new JsonException($"Invalid decimal format in '{fieldName}': expected a string or number.");
    }

    string decimalString = reader.GetString()?.Trim() ?? throw new JsonException($"'{fieldName}' value is missing.");

    if (decimal.TryParse(decimalString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal parsedDecimal)) {
      return parsedDecimal;
    }

    throw new JsonException($"Invalid decimal format in '{fieldName}': '{decimalString}'. Expected a valid numeric value.");
  }

  /// <summary>
  /// Converts a decimal object back into a string for a json response, will ensure it has 2 decimal places
  /// </summary>
  /// <param name="writer"></param>
  /// <param name="value"></param>
  /// <param name="options"></param>
  public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString("F2", CultureInfo.InvariantCulture)); // Ensure two decimal places
  }

      public static string GetPropertyName(ref Utf8JsonReader reader) {
    if (reader.TokenType == JsonTokenType.PropertyName) {
      string? fieldName = reader.GetString();
      reader.Read(); // Move to the value token
      return fieldName ?? "unknown_field";
    }
    return "unknown_field";
  }
}
