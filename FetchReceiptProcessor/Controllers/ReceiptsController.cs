using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("receipts")]
public class ReceiptsController : ControllerBase {
  private readonly IReceiptService _receiptService;

  public ReceiptsController(IReceiptService receiptService) {
    _receiptService = receiptService;
  }

  [HttpPost("process")]
  public IActionResult ProcessReceipt([FromBody] Receipt receipt) {
    try {
      if (receipt == null) {
        return BadRequest(new { message = "Invalid receipt data." });
      }

      var receiptId = _receiptService.ProcessReceipt(receipt);
      return Ok(new { id = receiptId });
    } catch (ReceiptValidationException ex) {
      return BadRequest(new { message = ex.Message, errors = ex.ValidationErrors });
    } catch (Exception ex) {
      Console.WriteLine($"Error processing receipt: {ex.Message}");
      return StatusCode(500, new { message = "An error occurred while processing the receipt." });
    }
  }

  [HttpGet("{id}/points")]
  public IActionResult GetReceiptPoints(Guid id) {
    try {
      var receiptPoints = _receiptService.GetPointsForReceiptId(id);
      return Ok(new { points = receiptPoints });
    } catch (ReceiptNotFoundException ex) {
      return NotFound(ex.Message);
    } catch (ReceiptCalculationException ex) {
      return BadRequest(ex.Message);
    } catch (Exception ex) {
      Console.WriteLine($"Error retrieving receipt points: {ex.Message}");
      return StatusCode(500, new { message = "An error occurred while retrieving receipt points." });
    }
  }
}
