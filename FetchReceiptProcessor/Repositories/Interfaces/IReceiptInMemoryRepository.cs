/// <summary>
/// In memory data "repository" solution for saving receipts in a given session
/// </summary>
public interface IReceiptInMemoryRepository {
  /// <summary>
  /// Add a receipt object to an in memory dictionary, create a new ID associated with it
  /// </summary>
  /// <param name="receipt"></param>
  /// <returns>Guid</returns>
  Guid AddReceipt(Receipt receipt);

  /// <summary>
  /// Return a receipt object given a specific id 
  /// </summary>
  /// <param name="id"></param>
  /// <returns>Receipt object</returns>
  Receipt? GetReceipt(Guid receiptId);
}
