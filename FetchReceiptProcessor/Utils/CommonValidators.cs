/// <summary>
/// Common validators for data, returns an error message so each instance can use as needed
/// </summary>
// Note: I'm not sold on having a list of errors always being the way that these are handled,
// but there are bunch of ways to do it and I think the most important thing is app consistency
public static class CommonValidators {
  /// <summary>
  /// Validates that a string is not null, empty, or whitespace.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="fieldName"></param>
  /// <param name="errors"></param>
  public static void ValidateStringNotEmpty(string? value, string fieldName, List<string> errors) {
    if (string.IsNullOrWhiteSpace(value)) {
      errors.Add($"{fieldName} cannot be empty or whitespace.");
    }
  }

  /// <summary>
  /// Validates that a list is not null or empty.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="list"></param>
  /// <param name="fieldName"></param>
  /// <param name="errors"></param>
  public static void ValidateListNotEmpty<T>(List<T>? list, string fieldName, List<string> errors) {
    if (list == null || list.Count == 0) {
      errors.Add($"{fieldName} must contain at least one item.");
    }
  }

  /// <summary>
  /// Validates that a decimal value is greater than 0.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="fieldName"></param>
  /// <param name="errors"></param>
  public static void ValidateDecimalGreaterThanZero(decimal value, string fieldName, List<string> errors) {
    if (value <= 0) {
      errors.Add($"{fieldName} must be greater than 0.");
    }
  }
}
