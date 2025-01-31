using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

public class ReceiptServiceValidateReceiptTests {
  private readonly Mock<IReceiptInMemoryRepository> _mockRepository;
  private readonly Mock<ILogger<ReceiptService>> _mockLogger;
  private readonly ReceiptService _receiptService;

  public ReceiptServiceValidateReceiptTests() {
    _mockRepository = new Mock<IReceiptInMemoryRepository>();
    _mockLogger = new Mock<ILogger<ReceiptService>>();

    _receiptService = new ReceiptService(_mockRepository.Object, _mockLogger.Object);
  }
  
  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenRetailerIsEmpty() {
    var receipt = new Receipt {
      Retailer = "",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> { new ReceiptItem { ShortDescription = "Item", Price = 10.00m } }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Retailer cannot be empty", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenRetailerIsWhitespace() {
    var receipt = new Receipt {
      Retailer = "   ",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> { new ReceiptItem { ShortDescription = "Item", Price = 10.00m } }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Retailer cannot be empty", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenTotalIsZero() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = 0.00m, // Invalid total
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> { new ReceiptItem { ShortDescription = "Item", Price = 10.00m } }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Total must be greater than 0", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenTotalIsNegative() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = -10.00m, // Invalid total
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> { new ReceiptItem { ShortDescription = "Item", Price = 10.00m } }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Total must be greater than 0", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenTotalDoesNotMatchSumOfItems() {
    var receipt = new Receipt {
      Retailer = "StoreX",
      Total = 50.00m, // Incorrect total
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "Item 1", Price = 10.00m },
        new ReceiptItem { ShortDescription = "Item 2", Price = 15.00m }
      }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Total (50.00) does not match", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenItemsListIsEmpty() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem>() // No items
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Items must contain at least one item", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenItemsListIsNull() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = null // Null items list
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Items must contain at least one item", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenItemDescriptionIsEmpty() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "", Price = 5.00m } 
      }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("ShortDescription cannot be empty", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenItemDescriptionIsWhitespace() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "   ", Price = 5.00m } 
      }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("ShortDescription cannot be empty", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenItemPriceIsZero() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "Item", Price = 0.00m } 
      }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Price must be greater than 0", exception.Message);
  }

  [Fact]
  public void ValidateReceipt_ShouldThrowException_WhenItemPriceIsNegative() {
    var receipt = new Receipt {
      Retailer = "Store",
      Total = 10.00m,
      PurchaseDate = new DateOnly(2025, 1, 1),
      PurchaseTime = new TimeOnly(10, 00),
      Items = new List<ReceiptItem> {
        new ReceiptItem { ShortDescription = "Item", Price = -5.00m } 
      }
    };

    var exception = Assert.Throws<ReceiptValidationException>(() => ReceiptService.ValidateReceipt(receipt));

    Assert.Contains("Price must be greater than 0", exception.Message);
  }
}
