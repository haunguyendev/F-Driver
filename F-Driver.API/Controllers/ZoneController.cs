using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
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
    }
}
