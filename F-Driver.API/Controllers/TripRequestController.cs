using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using F_Driver.Service.BuisnessModels.QueryParameters;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
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
    }
}
