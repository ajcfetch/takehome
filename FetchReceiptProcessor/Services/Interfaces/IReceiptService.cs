/// <summary>
/// Service layer for receipts, it will handle data creating and reading from repo layers
/// add it will do logic for calculating points
/// </summary>
public interface IReceiptService {
  /// <summary>
  /// Validate and add a new receipt to storage
  /// </summary>
  /// <param name="receipt"></param>
  /// <returns>Guid</returns>
  Guid ProcessReceipt(Receipt receipt);

  /// <summary>
  /// Get the total points for a receipt that was added to storage
  /// </summary>
  /// <param name="receiptId"></param>
  /// <returns></returns>
  int GetPointsForReceiptId(Guid receiptId);
}
