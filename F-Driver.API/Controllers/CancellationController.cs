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
    public class CancellationController : ControllerBase
    {
        private readonly CancellationService _cancellationService;

        public CancellationController(CancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCancellations()
        {
            var cancellations = await _cancellationService.GetCancellations();
            return Ok(cancellations);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCancellation(int id)
        {
            var cancellation = await _cancellationService.GetCancellation(id);
            if (cancellation == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Cancellation is not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<CancellationResponse>.Succeed(new CancellationResponse { CancellationModel = cancellation }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCancellation([FromBody] CancellationRequest cancellationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdCancellation = await _cancellationService.CreateCancellation(cancellationModel.MapToModel());
            return Created();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCancellation(int id, CancellationRequest cancellationModel)
        {
            var updatedCancellation = await _cancellationService.UpdateCancellation(id, cancellationModel.MapToModel());
            if (updatedCancellation == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Cancellation is not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<CancellationResponse>.Succeed(new CancellationResponse { CancellationModel = updatedCancellation }));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCancellation(int id)
        {
            var isDeleted = await _cancellationService.DeleteCancellation(id);
            if (!isDeleted)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Cancellation is not found."));
                return NotFound(result);
            }
            return NoContent();
        }
    }
}
