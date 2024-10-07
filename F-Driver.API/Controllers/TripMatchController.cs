using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.BuisnessModels.QueryParameters;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripMatchController : ControllerBase
    {
        private readonly TripMatchService _tripMatchService;

        public TripMatchController(TripMatchService tripMatchService)
        {
            _tripMatchService = tripMatchService;
        }

        //get trip match with filter
        [HttpGet]
        public async Task<IActionResult> GetTripRequests([FromQuery] TripMatchQueryParameters parameters)
        {
            try
            {
                // Gọi phương thức từ service để lấy danh sách người dùng theo bộ lọc
                var tripMatches = await _tripMatchService.GetAllTripMatchesAsync(parameters);

                // Chuẩn bị đối tượng trả về theo kiểu phân trang
                var paginatedTripMatches = new PaginatedResult<TripMatchModel>
                {
                    Page = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalItems = tripMatches.TotalCount,
                    TotalPages = (int)Math.Ceiling(tripMatches.TotalCount / (double)parameters.PageSize),
                    Data = tripMatches.Items
                };

                // Trả về kết quả thành công
                return Ok(ApiResult<PaginatedResult<TripMatchModel>>.Succeed(paginatedTripMatches));
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }

        #region create trip match
        [HttpPost("create-trip-match")]
        [SwaggerOperation(
    Summary = "Create trip match request",
    Description = "Allows a driver to select a trip request of a passenger to create a pending trip match."
)]
        [SwaggerResponse(200, "Trip match created successfully", typeof(IActionResult))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Trip request or driver not found")]
        [SwaggerResponse(500, "An error occurred while creating the trip match")]
        public async Task<ActionResult<IActionResult>> CreateTripMatch([FromBody] TripMatchRequest request)
        {
            try
            {
                // Kiểm tra header Authorization
                if (!Request.Headers.TryGetValue("Authorization", out var token))
                {
                    return Unauthorized(ApiResult<string>.Error("Authorization header is missing or invalid."));
                }

                // Lấy token từ Authorization header
                token = token.ToString().Split()[1];

                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized(ApiResult<string>.Error("Authorization header is missing or invalid."));
                }

                // Giải mã token để lấy Driver ID
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var driverClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);

                if (driverClaim == null)
                {
                    return Unauthorized(ApiResult<string>.Error("Unauthorized: No driver ID found in token."));
                }

                var driverId = int.Parse(driverClaim.Value);

                
                var result = await _tripMatchService.CreateTripMatchAsync(request.TripRequestId, driverId);

                if (!result)
                {
                    return NotFound(ApiResult<string>.Error("Trip request or driver not found."));
                }

                return Ok(ApiResult<string>.Succeed("Trip match created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

        #endregion
        #region api update status trip match
        [HttpPost("update-trip-match-status")]
        [SwaggerOperation(
     Summary = "Update trip match status",
     Description = "Allows a passenger to accept or reject a trip match request."
 )]
        [SwaggerResponse(200, "Trip match status updated successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An error occurred while updating the trip match status")]
        public async Task<IActionResult> UpdateTripMatchStatus([FromBody] UpdateTripMatchStatusRequest request)
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

                // Gọi service để cập nhật trạng thái
                await _tripMatchService.UpdateTripMatchStatusAsync(request.TripMatchId, passengerId, request.Status);

                return Ok(ApiResult<string>.Succeed("Trip match status updated successfully."));
            }
            catch (EntryPointNotFoundException ex)
            {
                return NotFound(ApiResult<object>.Fail(ex));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

        #region api start trip for driver

        #endregion
        [HttpPost("start-trip")]
        [SwaggerOperation(
    Summary = "Start trip",
    Description = "Allows a driver to start a trip by setting the trip match status to 'InProgress'."
)]
        [SwaggerResponse(200, "Trip started successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Trip match not found")]
        [SwaggerResponse(500, "An error occurred while starting the trip")]
        public async Task<IActionResult> StartTrip([FromBody] StartTripRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();

                return BadRequest(ApiResult<Dictionary<string, string[]>>.Error(new Dictionary<string, string[]>
        {
            { "Errors", errors.ToArray() }
        }));
            }

            if(!Request.Headers.TryGetValue("Authorization",out var token))
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
                var driverClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);
                if (driverClaim == null)
                {
                    return Unauthorized(ApiResult<string>.Error("Unauthorized: No driver ID found in token."));
                    
                }
                var driverId = int.Parse(driverClaim.Value);
                await _tripMatchService.StartTripAsync(request.TripMatchId, driverId);
                return Ok(ApiResult<string>.Succeed("Trip started successfully!"));
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
            #endregion

        }
}
