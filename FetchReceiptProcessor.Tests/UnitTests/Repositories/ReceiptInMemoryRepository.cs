using System;
using Xunit;

public class ReceiptInMemoryRepositoryTests {
  private readonly ReceiptInMemoryRepository _repository;

  public ReceiptInMemoryRepositoryTests() {
    _repository = new ReceiptInMemoryRepository();
  }

  [Fact]
  public void AddReceipt_ShouldReturnUniqueId() {
    var receipt1 = new Receipt { Retailer = "Store A", Total = 50.00m };
    var receipt2 = new Receipt { Retailer = "Store B", Total = 75.00m };

    Guid id1 = _repository.AddReceipt(receipt1);
    Guid id2 = _repository.AddReceipt(receipt2);

    Assert.NotEqual(Guid.Empty, id1);
    Assert.NotEqual(Guid.Empty, id2);
    Assert.NotEqual(id1, id2);
  }

  [Fact]
  public void GetReceipt_ShouldReturnCorrectReceipt() {
    var receipt = new Receipt { Retailer = "Target", Total = 99.99m };
    Guid id = _repository.AddReceipt(receipt);

    var retrievedReceipt = _repository.GetReceipt(id);

    Assert.NotNull(retrievedReceipt);
    Assert.Equal(receipt.Retailer, retrievedReceipt.Retailer);
    Assert.Equal(receipt.Total, retrievedReceipt.Total);
  }

  [Fact]
  public void GetReceipt_WithInvalidId_ShouldReturnNull() {
    var receipt = _repository.GetReceipt(Guid.NewGuid());

    Assert.Null(receipt);
  }
}
