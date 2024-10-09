using F_Driver.API.Common;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{paymentId}/confirm")]
        [SwaggerOperation(
            Summary = "Confirm Payment",
            Description = "Allows a driver to confirm a payment, which will trigger the payment process if both parties agree."
        )]
        [SwaggerResponse(200, "Payment confirmed successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Payment not found")]
        [SwaggerResponse(500, "An error occurred while confirming the payment")]
        public async Task<IActionResult> ConfirmPayment(int paymentId)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token))
            {
                return Unauthorized(ApiResult<string>.Error("Authorization header is missing or invalid."));
            }

            token = token.ToString().Split()[1]; // Lấy token từ header Authorization
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(ApiResult<string>.Error("Authorization header is missing or invalid."));
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var driverClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);

                if (driverClaim == null)
                {
                    return Unauthorized(ApiResult<string>.Error("Unauthorized: No driver ID found in token."));
                }

                var driverId = int.Parse(driverClaim.Value); // Lấy driverId từ JWT token

                // Gọi service để xác nhận thanh toán
                var result = await _paymentService.ConfirmPaymentAsync(paymentId, driverId);

                if (result)
                {
                    return Ok(ApiResult<string>.Succeed("Payment confirmed successfully."));
                }

                return BadRequest(ApiResult<string>.Error("Failed to confirm payment."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResult<object>.Fail(ex));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResult<object>.Fail(ex));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

    }
}
