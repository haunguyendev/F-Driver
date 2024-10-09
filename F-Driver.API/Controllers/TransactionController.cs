using F_Driver.API.Common;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Helpers;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        #region api get history transaction 
        [HttpGet("history")]
        [SwaggerOperation(
    Summary = "Get Transaction History with Filters and Pagination",
    Description = "Fetches the transaction history of the logged-in user with filters and pagination support."
)]
        [SwaggerResponse(200, "Transaction history retrieved successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "User or transactions not found")]
        [SwaggerResponse(500, "An error occurred while retrieving the transaction history")]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] TransactionQueryParameters parameters)
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
                var userClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);

                if (userClaim == null)
                {
                    return Unauthorized(ApiResult<string>.Error("Unauthorized: No user ID found in token."));
                }

                var userId = int.Parse(userClaim.Value); // Lấy userId từ JWT token

                // Gọi service để lấy lịch sử giao dịch của người dùng với phân trang và lọc
                var paginatedTransactions = await _transactionService.GetTransactionsByUserIdAsync(userId, parameters);

                if (paginatedTransactions == null || !paginatedTransactions.Items.Any())
                {
                    return NotFound(ApiResult<string>.Error("No transactions found for this user."));
                }

                return Ok(ApiResult<PaginatedList<TransactionResponseModel>>.Succeed(paginatedTransactions));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResult<object>.Fail(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

        #endregion
    }
}
