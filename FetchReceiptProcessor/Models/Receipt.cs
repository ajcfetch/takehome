using System.Text.Json.Serialization;

public class Receipt {
  // Note: This is a field I would ideally have mapped to an object or id that represents
  // a recognized retailer, and if it's one we didn't have recognized in the system, I would
  // assume we would want a system to save it and have it flagged as something for a team 
  // to review. For the purposes of this exercise I'll leave it as a string and let that 
  // be it's identifier. 
  public required string Retailer { get; set; }

  [JsonConverter(typeof(DateOnlyJsonConverter))]
  public DateOnly PurchaseDate { get; set; }

  // Note: I'm still not sure about this decision and if I'd rather combine the 
  // json data into a singular datetime, I feel I could be convinced either way
  [JsonConverter(typeof(TimeOnlyJsonConverter))]
  public TimeOnly PurchaseTime { get; set; }

  public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();

  [JsonConverter(typeof(DecimalJsonConverter))]
  public decimal Total { get; set; }
}
