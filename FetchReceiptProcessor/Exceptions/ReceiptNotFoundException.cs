public class ReceiptNotFoundException : Exception {
  public ReceiptNotFoundException(Guid receiptId) 
    : base($"No receipt found for that ID: {receiptId}")
  {
  }
}
