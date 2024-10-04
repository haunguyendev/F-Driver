using F_Driver.API.Common;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.BuisnessModels.QueryParameters;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
