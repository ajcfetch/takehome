public class ReceiptValidationException : Exception {
  public List<string> ValidationErrors { get; }

  public ReceiptValidationException(List<string> validationErrors)
    : base($"Receipt validation failed: {string.Join(" | ", validationErrors)}")
  {
      ValidationErrors = validationErrors ?? new List<string>();
  }
}
