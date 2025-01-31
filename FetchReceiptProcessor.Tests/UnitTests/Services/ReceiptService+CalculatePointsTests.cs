using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

public class ReceiptServiceCalculatePointsTests {
  private readonly Mock<IReceiptInMemoryRepository> _mockRepository;
  private readonly Mock<ILogger<ReceiptService>> _mockLogger;
  private readonly ReceiptService _receiptService;

  public ReceiptServiceCalculatePointsTests() {
    _mockRepository = new Mock<IReceiptInMemoryRepository>();
    _mockLogger = new Mock<ILogger<ReceiptService>>();

    _receiptService = new ReceiptService(_mockRepository.Object, _mockLogger.Object);
  }

  [Fact]
  public void CalculatePoints_ShouldMatchExample1_TargetReceipt() {
    var receipt = new Receipt {
      Retailer = "Target",
      Total = 35.35m,
      PurchaseDate = new DateOnly(2022, 1, 1), // Odd day
      PurchaseTime = new TimeOnly(13, 01), // Not between 2-4 PM
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "Mountain Dew 12PK", Price = 6.49m },
        new ReceiptItem { ShortDescription = "Emils Cheese Pizza", Price = 12.25m }, // Length 18 (multiple of 3)
        new ReceiptItem { ShortDescription = "Knorr Creamy Chicken", Price = 1.26m },
        new ReceiptItem { ShortDescription = "Doritos Nacho Cheese", Price = 3.35m },
        new ReceiptItem { ShortDescription = "   Klarbrunn 12-PK 12 FL OZ  ", Price = 12.00m } // Length 24 (multiple of 3)
      }
    };
    var receiptId = Guid.NewGuid();

    int points = _receiptService.CalculatePoints(receipt, receiptId);

    Assert.Equal(28, points);
  }

  [Fact]
  public void CalculatePoints_ShouldMatchExample2_MandMCornerMarketReceipt() {
    var receipt = new Receipt {
      Retailer = "M&M Corner Market",
      Total = 9.00m, // Round dollar + multiple of 0.25
      PurchaseDate = new DateOnly(2022, 3, 20), // Even day (no odd day bonus)
      PurchaseTime = new TimeOnly(14, 33), // Between 2-4 PM
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "Gatorade", Price = 2.25m },
        new ReceiptItem { ShortDescription = "Gatorade", Price = 2.25m },
        new ReceiptItem { ShortDescription = "Gatorade", Price = 2.25m },
        new ReceiptItem { ShortDescription = "Gatorade", Price = 2.25m }
      }
    };
    var receiptId = Guid.NewGuid();

    int points = _receiptService.CalculatePoints(receipt, receiptId);

    Assert.Equal(109, points);
  }
}
