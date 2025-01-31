public class ReceiptService : IReceiptService {
  private readonly IReceiptInMemoryRepository _receiptRepository;
  private readonly ILogger<ReceiptService> _logger;

  public ReceiptService(IReceiptInMemoryRepository receiptRepository, ILogger<ReceiptService> logger) {
    _receiptRepository = receiptRepository;
    _logger = logger;
  }

  public Guid ProcessReceipt(Receipt receipt) {
    _logger.LogInformation("Processing receipt for retailer: {Retailer}", receipt.Retailer);
    ValidateReceipt(receipt);
    Guid receiptId = _receiptRepository.AddReceipt(receipt);
    _logger.LogInformation("Receipt processed successfully with ID: {ReceiptId}", receiptId);
    return receiptId;
  }

  public int GetPointsForReceiptId(Guid receiptId) {
    _logger.LogInformation("Fetching points for receipt ID: {ReceiptId}", receiptId);
    var receipt = _receiptRepository.GetReceipt(receiptId);
    if (receipt == null) {
      _logger.LogWarning("Receipt not found for ID: {ReceiptId}", receiptId);
      throw new ReceiptNotFoundException(receiptId);
    }

    int points = CalculatePoints(receipt, receiptId);
    _logger.LogInformation("Total points for receipt {ReceiptId}: {Points}", receiptId, points);
    return points;
  }

  internal int CalculatePoints(Receipt receipt, Guid receiptId) {
    try {
      int totalPoints = 0;
      var breakdown = new List<string>();

      _logger.LogInformation("Calculating points for receipt ID: {ReceiptId}", receiptId);

      // 1️⃣ One point per alphanumeric character in retailer name
      int retailerPoints = receipt.Retailer.Count(char.IsLetterOrDigit);
      totalPoints += retailerPoints;
      breakdown.Add($"{retailerPoints} points - retailer name has {retailerPoints} alphanumeric characters");

      // 2️⃣ 50 points if total is a round dollar amount
      if (receipt.Total % 1 == 0) {
        totalPoints += 50;
        breakdown.Add("50 points - total is a round dollar amount");
      }

      // 3️⃣ 25 points if total is a multiple of 0.25
      if (receipt.Total % 0.25m == 0) {
        totalPoints += 25;
        breakdown.Add("25 points - total is a multiple of 0.25");
      }

      // 4️⃣ 5 points for every two items
      int itemPairPoints = (receipt.Items.Count / 2) * 5;
      totalPoints += itemPairPoints;
      breakdown.Add($"{itemPairPoints} points - {receipt.Items.Count} items ({receipt.Items.Count / 2} pairs @ 5 points each)");

      // 5️⃣ If item description length is a multiple of 3, add extra points
      int descriptionPoints = 0;
      foreach (var item in receipt.Items) {
        int length = item.ShortDescription.Trim().Length;
        if (length % 3 == 0) {
          int extraPoints = (int)Math.Ceiling(item.Price * 0.2m);
          descriptionPoints += extraPoints;
          breakdown.Add($"{extraPoints} points - \"{item.ShortDescription.Trim()}\" is {length} characters (a multiple of 3)");
          breakdown.Add($"              item price of {item.Price:F2} * 0.2 = {item.Price * 0.2m:F2}, rounded up is {extraPoints} points");
        }
      }
      totalPoints += descriptionPoints;

      // 6️⃣ 6 points if purchase date is an odd day
      if (receipt.PurchaseDate.Day % 2 == 1) {
        totalPoints += 6;
        breakdown.Add("6 points - purchase day is odd");
      }

      // 7️⃣ 10 points if time is between 2:00 PM and 4:00 PM
      if (receipt.PurchaseTime.Hour >= 14 && receipt.PurchaseTime.Hour < 16) {
        totalPoints += 10;
        breakdown.Add($"10 points - {receipt.PurchaseTime:HH:mm} is between 2:00pm and 4:00pm");
      }

      // 📝 Log the full breakdown
      _logger.LogInformation("Total Points: {TotalPoints}", totalPoints);
      _logger.LogInformation("Breakdown:\n{Breakdown}\n  + ---------\n  = {TotalPoints} points",
        string.Join("\n", breakdown), totalPoints);

      return totalPoints;
    } catch (Exception ex) {
      _logger.LogError(ex, "Failed to calculate receipt points for {ReceiptId}", receiptId);
      throw new ReceiptCalculationException($"Failed to calculate receipt points for {receiptId}", ex);
    }
  }

  internal static void ValidateReceipt(Receipt receipt) {
    var errors = new List<string>();

    CommonValidators.ValidateStringNotEmpty(receipt.Retailer, nameof(receipt.Retailer), errors);
    CommonValidators.ValidateListNotEmpty(receipt.Items, nameof(receipt.Items), errors);
    CommonValidators.ValidateDecimalGreaterThanZero(receipt.Total, nameof(receipt.Total), errors);

    if (receipt.Items == null || receipt.Items.Count == 0) {
      errors.Add("Items must contain at least one item.");
    } else {
      foreach (var item in receipt.Items) {
        CommonValidators.ValidateStringNotEmpty(item.ShortDescription, $"{nameof(receipt.Items)}[{receipt.Items.IndexOf(item)}].{nameof(item.ShortDescription)}", errors);
        CommonValidators.ValidateDecimalGreaterThanZero(item.Price, $"{nameof(receipt.Items)}[{receipt.Items.IndexOf(item)}].{nameof(item.Price)}", errors);
      }

      decimal calculatedTotal = receipt.Items.Sum(item => item.Price);
      if (Math.Abs(receipt.Total - calculatedTotal) > 0.01m) {
        errors.Add($"Total ({receipt.Total}) does not match the sum of item prices ({calculatedTotal}).");
      }
    }

    if (errors.Count > 0) {
      throw new ReceiptValidationException(errors);
    }
  }
}
