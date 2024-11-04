using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.DataAccessObject.Models;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace F_Driver.API.Controllers
{
    [Route("api/zones")]
    [ApiController]
    public class ZoneController : ControllerBase
    {

        private readonly ZoneService _zoneService;

        public ZoneController(ZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        //Get zone by id
        [HttpGet("{zoneId}")]
        public async Task<IActionResult> GetZoneById(int zoneId)
        {
            var zone = await _zoneService.GetZoneById(zoneId);
            if (zone == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Zone not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<ZoneResponse>.Succeed(new ZoneResponse { Zone = zone }));

        }

        //Create zone
        [HttpPost]
        public async Task<IActionResult> CreateNewZone([FromBody] ZoneRequestModel zoneRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var zone = await _zoneService.CreateZone(zoneRequest.MapToZoneModel());
            if (!zone)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Create zone is unsuccessful because some error or Zone name is exist"));
                return BadRequest(result);
            }
            return Created();
        }

        //Update zone
        [HttpPut("{zoneId}")]
        public async Task<IActionResult> UpdateZone(int zoneId, [FromBody] ZoneRequestModel zoneRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var zone = await _zoneService.UpdateZone(zoneId, zoneRequest.MapToZoneModel());
            if (zone == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Zone not found or Zone name is exist"));
                return NotFound(result);
            }
            return Ok(ApiResult<ZoneResponse>.Succeed(new ZoneResponse { Zone = zone }));
        }

        [HttpGet("filter")]
        //Get list zone by from zone id or to zone id
        public async Task<IActionResult> GetListZoneByFromZoneIdOrToZoneId(int? fromZoneId, int? toZoneId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (fromZoneId == null && toZoneId == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("From zone id or To zone id is required."));
                return BadRequest(result);
            }
            if(fromZoneId != null && toZoneId != null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("From zone id and To zone id cannot be used at the same time."));
                return BadRequest(result);
            }
            var zones = await _zoneService.GetListZoneByFromZoneIdOrToZoneId(fromZoneId, toZoneId);
            if (zones == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Zone not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<ZonesResponse>.Succeed(new ZonesResponse { Zones = zones }));
        }

        //get all zone with filter
        [HttpGet]
        public async Task<IActionResult> GetListZone([FromQuery] ZoneQueryParameters zoneQueryParameters)
        {
            try
            {
                // Gọi phương thức từ service để lấy danh sách người dùng theo bộ lọc
                var zones = await _zoneService.GetAllZonesAsync(zoneQueryParameters);

                // Chuẩn bị đối tượng trả về theo kiểu phân trang
                var paginatedZones = new PaginatedResult<ZoneModel>
                {
                    Page = zoneQueryParameters.Page,
                    PageSize = zoneQueryParameters.PageSize,
                    TotalItems = zones.TotalCount,
                    TotalPages = (int)Math.Ceiling(zones.TotalCount / (double)zoneQueryParameters.PageSize),
                    Data = zones.Items
                };

                // Trả về kết quả thành công
                return Ok(ApiResult<PaginatedResult<ZoneModel>>.Succeed(paginatedZones));
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }
        // api delete zone
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a zone"
     
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Zone deleted successfully", typeof(ApiResult<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Zone not found", typeof(ApiResult<object>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while deleting the Zone", typeof(ApiResult<object>))]
        public async Task<IActionResult> DeleteZone([FromRoute] int id)
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

            try
            {
                var deleteResult = await _zoneService.DeleteZoneAsync(id);

                if (!deleteResult)
                {
                    return NotFound(ApiResult<object>.Error(new { Message = "Category not found" }));
                }

                return Ok(ApiResult<object>.Succeed(new { Message = "Category deleted successfully" }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }
    }
}
