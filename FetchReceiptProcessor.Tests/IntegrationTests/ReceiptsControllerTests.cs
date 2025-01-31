using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ReceiptsControllerTests : IClassFixture<WebApplicationFactory<Program>> {
  private readonly HttpClient _client;

  public ReceiptsControllerTests(WebApplicationFactory<Program> factory) {
    _client = factory.CreateClient();
  }

  private static StringContent GetJsonContent(object obj) {
    return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
  }

  // Process a valid receipt and ensure it returns an ID
  [Fact]
  public async Task ProcessReceipt_ShouldReturnReceiptId_WhenValid() {
    var receipt = new {
      retailer = "Target",
      purchaseDate = "2022-01-01",
      purchaseTime = "13:01",
      items = new[] {
        new { shortDescription = "Mountain Dew 12PK", price = "6.49" },
        new { shortDescription = "Emils Cheese Pizza", price = "12.25" }
      },
      total = "18.74"
    };

    var response = await _client.PostAsync("/receipts/process", GetJsonContent(receipt));
    
    var responseBody = await response.Content.ReadAsStringAsync();
    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.True(Guid.TryParse(jsonResponse.GetProperty("id").GetString(), out _));
  }

  // Processing an invalid receipt should return 400 BadRequest
  [Fact]
  public async Task ProcessReceipt_ShouldReturnBadRequest_WhenInvalid() {
    var receipt = new {
      retailer = "", // Invalid retailer
      purchaseDate = "2022-01-01",
      purchaseTime = "13:01",
      items = new object[] {}, // No items
      total = "0.00"
    };

    var response = await _client.PostAsync("/receipts/process", GetJsonContent(receipt));
    
    var responseBody = await response.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    Assert.Contains("Retailer cannot be empty", responseBody);
    Assert.Contains("Items must contain at least one item", responseBody);
    Assert.Contains("Total must be greater than 0", responseBody);
  }

  // Retrieve points for an existing receipt
  [Fact]
  public async Task GetReceiptPoints_ShouldReturnPoints_WhenReceiptExists() {
    var receipt = new {
      retailer = "Target",
      purchaseDate = "2022-01-01",
      purchaseTime = "13:01",
      items = new[] {
        new { shortDescription = "Mountain Dew 12PK", price = "6.49" },
        new { shortDescription = "Emils Cheese Pizza", price = "12.25" }
      },
      total = "18.74"
    };

    // Step 1: Post the receipt
    var postResponse = await _client.PostAsync("/receipts/process", GetJsonContent(receipt));
    var postBody = await postResponse.Content.ReadAsStringAsync();
    var receiptId = JsonSerializer.Deserialize<JsonElement>(postBody).GetProperty("id").GetString();

    // Step 2: Fetch points using the returned ID
    var getResponse = await _client.GetAsync($"/receipts/{receiptId}/points");
    
    var getResponseBody = await getResponse.Content.ReadAsStringAsync();
    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(getResponseBody);

    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    Assert.True(jsonResponse.TryGetProperty("points", out _));
  }

  // Requesting points for a non-existent receipt should return 404
  [Fact]
  public async Task GetReceiptPoints_ShouldReturnNotFound_WhenReceiptDoesNotExist() {
    var nonExistentId = Guid.NewGuid();

    var response = await _client.GetAsync($"/receipts/{nonExistentId}/points");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}
