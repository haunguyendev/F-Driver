using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.BuisnessModels.QueryParameters;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Helpers;
using F_Driver.Service.Services;
using F_Driver.Service.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;

namespace F_Driver.API.Controllers
{
    [Route("api/tripmatchs")]
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
        [SwaggerResponse(200, "Price tables retrieved successfully", typeof(PaginatedList<TripMatchReponseModel>))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(500, "An error occurred while retrieving price tables")]
        public async Task<IActionResult> GetTripRequests([FromQuery] TripMatchQueryParameters parameters)
        {
            try
            {
                // Gọi phương thức từ service để lấy danh sách người dùng theo bộ lọc
                var tripMatches = await _tripMatchService.GetAllTripMatchesAsync(parameters);

                // Chuẩn bị đối tượng trả về theo kiểu phân trang
                var paginatedTripMatches = new PaginatedResult<TripMatchReponseModel>
                {
                    Page = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalItems = tripMatches.TotalCount,
                    TotalPages = (int)Math.Ceiling(tripMatches.TotalCount / (double)parameters.PageSize),
                    Data = tripMatches.Items
                };

                // Trả về kết quả thành công
                return Ok(ApiResult<PaginatedResult<TripMatchReponseModel>>.Succeed(paginatedTripMatches));
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }

        //api get by id 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripMatchById([FromRoute] int id)
        {
            try
            {
                var tripmatches = await _tripMatchService.GetTripMatchById(id);
                return Ok(ApiResult<TripMatchReponseModel>.Succeed(tripmatches));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }
        

        #region create trip match
        [HttpPost()]
        [SwaggerOperation(
    Summary = "Create trip match request",
    Description = "Allows a driver to select a trip request of a passenger to create a pending trip match."
)]
        [SwaggerResponse(200, "Trip match created successfully", typeof(IActionResult))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Trip request or driver not found")]
        [SwaggerResponse(500, "An error occurred while creating the trip match")]
        public async Task<ActionResult> CreateTripMatch([FromBody] TripMatchRequest request)
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






        #region api update tripmatch status
        [HttpPut("{id}/status")]
        [SwaggerOperation(
            Summary = "Update trip or trip match status",
            Description = "Allows a driver or passenger to update the status of a trip (start, complete, or cancel) or trip match (accept, reject)."
        )]
        [SwaggerResponse(200, "Trip or trip match status updated successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Trip or trip match not found")]
        [SwaggerResponse(500, "An error occurred while updating the trip or trip match status")]
        public async Task<IActionResult> UpdateTripStatus([FromBody] UpdateTripStatusRequest request, [FromRoute] int id)
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
                var userClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);

                if (userClaim == null)
                {
                    return Unauthorized(ApiResult<string>.Error("Unauthorized: No user ID found in token."));
                }

                var userId = int.Parse(userClaim.Value);

                // Kiểm tra nếu yêu cầu là cập nhật trạng thái ghép chuyến
                if (request.IsTripMatchUpdate)
                {
                    // Gọi service để cập nhật trạng thái ghép chuyến
                   if(request.Status==TripMatchStatusEnum.Accepted||request.Status==TripMatchStatusEnum.Rejected)
                    {
                        await _tripMatchService.UpdateTripMatchStatusAsync(id, userId, request.Status);
                    }
                    else {
                        return BadRequest(ApiResult<string>.Error("Invalid trip status."));
                    }

                    return Ok(ApiResult<string>.Succeed("Trip match status updated successfully."));
                }
                else
                {
                    // Gọi service để cập nhật trạng thái chuyến đi
                    switch (request.Status)
                    {
                        case TripMatchStatusEnum.InProgress:
                            await _tripMatchService.StartTripAsync(id, userId);
                            break;

                        case TripMatchStatusEnum.Completed:
                            await _tripMatchService.CompleteTripAsync(id, userId);
                            break;

                        case TripMatchStatusEnum.Canceled:
                            await _tripMatchService.CancelTripMatchAsync(id, (int)request.ReasonId, userId);
                            break;

                        default:
                            return BadRequest(ApiResult<string>.Error("Invalid trip status."));
                    }

                    return Ok(ApiResult<string>.Succeed("Trip status updated successfully."));
                }
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
