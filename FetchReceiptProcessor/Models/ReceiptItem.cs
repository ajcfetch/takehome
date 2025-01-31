using System.Text.Json.Serialization;

public class ReceiptItem {
  public required string ShortDescription { get; set; }

  [JsonConverter(typeof(DecimalJsonConverter))]
  public decimal Price { get; set; }
}
