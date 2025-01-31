using System.Collections.Concurrent;

public class ReceiptInMemoryRepository : IReceiptInMemoryRepository {
  // Note: for the sake of simplicity for this project, using a concurrent dictionary
  // is a quick option for in memory storage. Normally I would have a database specific
  // receipt model that includes id and other data like the datetime it was added, by who, etc
  private readonly ConcurrentDictionary<Guid, Receipt> _receipts = new();

  public Guid AddReceipt(Receipt receipt) {
    Guid id = Guid.NewGuid();
    _receipts[id] = receipt;
    return id;
  }

  public Receipt? GetReceipt(Guid id) {
    _receipts.TryGetValue(id, out var receipt);
    return receipt;
  }
}
