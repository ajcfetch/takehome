using System;
using System.Collections.Generic;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;

public class ReceiptServiceTests {
  private readonly Mock<IReceiptInMemoryRepository> _mockRepository;
  private readonly Mock<ILogger<ReceiptService>> _mockLogger;
  private readonly ReceiptService _receiptService;

  public ReceiptServiceTests() {
    _mockRepository = new Mock<IReceiptInMemoryRepository>();
    _mockLogger = new Mock<ILogger<ReceiptService>>();

    _receiptService = new ReceiptService(_mockRepository.Object, _mockLogger.Object);
  }

  [Fact]
  public void ProcessReceipt_ShouldReturnValidGuid_WhenReceiptIsValid() {
    var receipt = new Receipt {
      Retailer = "Target",
      Total = 35.35m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(14, 30),
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "Item 1", Price = 10.00m },
        new ReceiptItem { ShortDescription = "Item 2", Price = 25.35m }
      }
    };
    var fakeGuid = Guid.NewGuid();
    
    _mockRepository.Setup(r => r.AddReceipt(It.IsAny<Receipt>())).Returns(fakeGuid);

    Guid result = _receiptService.ProcessReceipt(receipt);

    Assert.Equal(fakeGuid, result);
    _mockRepository.Verify(r => r.AddReceipt(It.IsAny<Receipt>()), Times.Once);
  }

  [Fact]
  public void ProcessReceipt_ShouldThrowValidationException_WhenReceiptIsInvalid() {
    var invalidReceipt = new Receipt {
      Retailer = "", // Invalid (empty retailer)
      Total = 0, // Invalid (total cannot be zero)
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem>()
    };

    var exception = Assert.Throws<ReceiptValidationException>(() =>
      _receiptService.ProcessReceipt(invalidReceipt));

    Assert.Contains("Retailer cannot be empty", exception.Message);
    Assert.Contains("Items must contain at least one item", exception.Message);
    Assert.Contains("Total must be greater than 0", exception.Message);
  }

  [Fact]
  public void GetPointsForReceiptId_ShouldReturnCorrectPoints_WhenReceiptExists() {
    var receipt = new Receipt {
      Retailer = "StoreX",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 5),
      PurchaseTime = new TimeOnly(15, 00),
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "Product ABC", Price = 5.00m },
        new ReceiptItem { ShortDescription = "XYZ", Price = 5.00m }
      }
    };
    var receiptId = Guid.NewGuid();

    _mockRepository.Setup(r => r.GetReceipt(receiptId)).Returns(receipt);

    int points = _receiptService.GetPointsForReceiptId(receiptId);

    Assert.True(points > 0); 
    _mockRepository.Verify(r => r.GetReceipt(receiptId), Times.Once);
  }

  [Fact]
  public void GetPointsForReceiptId_ShouldThrowNotFoundException_WhenReceiptDoesNotExist() {
    var invalidReceiptId = Guid.NewGuid();

    _mockRepository.Setup(r => r.GetReceipt(invalidReceiptId)).Returns((Receipt?)null);

    var exception = Assert.Throws<ReceiptNotFoundException>(() =>
      _receiptService.GetPointsForReceiptId(invalidReceiptId));

    Assert.Contains("No receipt found for that ID", exception.Message);
  }
}
