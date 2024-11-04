using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using F_Driver.Service.BuisnessModels.QueryParameters;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;

namespace F_Driver.API.Controllers
{
    [Route("api/triprequests")]
    [ApiController]
    public class TripRequestController : ControllerBase
    {
        private readonly TripRequestService _tripRequestService;

        public TripRequestController(TripRequestService tripRequestService)
        {
            _tripRequestService = tripRequestService;
        }

        //Create trip request
        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] TripRequestRequest tripRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var checkWalet = await _tripRequestService.CheckWallet(tripRequestModel.MapToTripRequestModel());
            if (!checkWalet)
            {
                return BadRequest(ApiResult<string>.Error("Not enough balance to create trip request."));
            }
            var tripRequest = await _tripRequestService.CreateTripRequest(tripRequestModel.MapToTripRequestModel());
            if (!tripRequest)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Create trip request is unsuccessful."));
                return BadRequest(result);
            }
            return Created();
        }

        [HttpGet]
        public async Task<IActionResult> GetTripRequests([FromQuery] TripRequestQueryParameters parameters)
        {
            try
            {
                // Gọi phương thức từ service để lấy danh sách người dùng theo bộ lọc
                var tripRequests = await _tripRequestService.GetTripRequests(parameters);

                // Chuẩn bị đối tượng trả về theo kiểu phân trang
                var paginatedTripRequests = new PaginatedResult<TripRequestModel>
                {
                    Page = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalItems = tripRequests.TotalCount,
                    TotalPages = (int)Math.Ceiling(tripRequests.TotalCount / (double)parameters.PageSize),
                    Data = tripRequests.Items
                };

                // Trả về kết quả thành công
                return Ok(ApiResult<PaginatedResult<TripRequestModel>>.Succeed(paginatedTripRequests));
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }

        #region api passenger cancel trip request
        [HttpPut("{id}")]
        [SwaggerOperation(
    Summary = "Cancel trip request",
    Description = "Allows a passenger to cancel a trip request before a trip match is made."
)]
        [SwaggerResponse(200, "Trip request canceled successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Trip request not found")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An error occurred while canceling the trip request")]
        public async Task<IActionResult> CancelTripRequest([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(ApiResult<Dictionary<string, string[]>>.Error(new Dictionary<string, string[]>
        {
            { "Errors", errors.ToArray() }
        }));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var token))
            {
                return Unauthorized(ApiResult<string>.Error("Authorization header is missing or invalid."));
            }

            token = token.ToString().Split()[1];
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(ApiResult<string>.Error("Authorization header is missing or invalid."));
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var passengerClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);

                if (passengerClaim == null)
                {
                    return Unauthorized(ApiResult<string>.Error("Unauthorized: No passenger ID found in token."));
                }

                var passengerId = int.Parse(passengerClaim.Value);

                // Gọi service để hủy yêu cầu với passengerId
                await _tripRequestService.CancelTripRequestAsync(id, passengerId);

                return Ok(ApiResult<string>.Succeed("Trip request canceled successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResult<object>.Fail(ex));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResult<object>.Fail(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

        #endregion
    }
}
