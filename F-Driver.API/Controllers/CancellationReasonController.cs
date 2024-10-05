using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.DataAccessObject.Models;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationReasonController : ControllerBase
    {
        private readonly CancellationReasonService _cancellationReasonService;

        public CancellationReasonController(CancellationReasonService cancellationReasonService)
        {
            _cancellationReasonService = cancellationReasonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCancellationReasons()
        {
            var cancellationReasons = await _cancellationReasonService.GetCancellationReasons();
            return Ok(cancellationReasons);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCancellationReason(int id)
        {
            var cancellationReason = await _cancellationReasonService.GetCancellationReason(id);
            if (cancellationReason == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Cancellation is not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<CancellationReasonResponse>.Succeed(new CancellationReasonResponse { CancellationReasonModel = cancellationReason }));
        }
        [HttpPost]
        public async Task<IActionResult> CreateCancellationReason([FromBody] CancellationReasonRequest cancellationReasonModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdCancellationReason = await _cancellationReasonService.CreateCancellationReason(cancellationReasonModel.MapToModel());
            return Created();
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCancellationReason(int id, CancellationReasonRequest cancellationReasonModel)
        {
            var updatedCancellationReason = await _cancellationReasonService.UpdateCancellationReason(id, cancellationReasonModel.MapToModel());
            if (updatedCancellationReason == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Cancellation is not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<CancellationReasonResponse>.Succeed(new CancellationReasonResponse { CancellationReasonModel = updatedCancellationReason }));
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCancellationReason(int id)
        {
            var isDeleted = await _cancellationReasonService.DeleteCancellationReason(id);
            if (!isDeleted)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Cancellation is not found or in use"));
                return NotFound(result);
            }
            return NoContent();
        }
    }
}
