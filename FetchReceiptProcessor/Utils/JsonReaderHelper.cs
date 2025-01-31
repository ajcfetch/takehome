using System.Text.Json;

public static class JsonReaderHelper {
  /// <summary>
  /// Move back one step to read the property name to make debugging easier and error messages more clear
  /// </summary>
  /// <param name="reader"></param>
  /// <returns></returns>
  public static string GetPropertyName(ref Utf8JsonReader reader) {
    if (reader.TokenType == JsonTokenType.PropertyName) {
      string? fieldName = reader.GetString();
      reader.Read(); // Move to the value token
      return fieldName ?? "unknown_field";
    }
    return "unknown_field";
  }
}
