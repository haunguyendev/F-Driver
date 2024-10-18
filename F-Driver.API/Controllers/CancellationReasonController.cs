using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.DataAccessObject.Models;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace F_Driver.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CancellationReasonController : ControllerBase
    {
        private CancellationReasonService _service;
        public CancellationReasonController(CancellationReasonService service)
        {
            _service = service;

        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCancellationReason(int id)
        {
            var cancellationReason = await _service.GetCancellationReason(id);
            if (cancellationReason == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Cancellation is not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<CancellationReasonResponse>.Succeed(new CancellationReasonResponse { ReasonId = cancellationReason.Id, Content = cancellationReason.Content }));
        }
        [HttpGet("CancellationReasons")]
        [SwaggerOperation(
        Summary = "Get all cancellation reasons",
        Description = "Fetches a list of all cancellation reasons available for trip cancellation."
    )]
        [SwaggerResponse(200, "Cancellation reasons retrieved successfully", typeof(ApiResult<List<CancellationReasonResponse>>))]
        [SwaggerResponse(500, "An error occurred while fetching cancellation reasons")]
        public async Task<IActionResult> GetAllCancellationReasons()
        {
            try
            {
                var reasons = await _service.GetAllCancellationReasonsAsync();
                var response = reasons.Select(r => new CancellationReasonResponse
                {
                    ReasonId = r.Id,
                    Content = r.Content
                }).ToList();

                return Ok(ApiResult<List<CancellationReasonResponse>>.Succeed(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

        #region api create 
        [HttpPost("CancellationReasons")]
        [SwaggerOperation(
        Summary = "Create new cancellation reason",
        Description = "Creates a new cancellation reason which can be used for cancelling a trip."
    )]
        [SwaggerResponse(200, "Cancellation reason created successfully", typeof(ApiResult<CancellationReasonResponse>))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(500, "An error occurred while creating the cancellation reason")]
        public async Task<IActionResult> CreateCancellationReason([FromBody] CreateCancellationReasonRequest request)
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
                var cancellationReason = new CancellationReason
                {
                    Content = request.Content
                };

                await _service.CreateCancellationReasonAsync(cancellationReason);

                var response = new CancellationReasonResponse
                {
                    ReasonId = cancellationReason.Id,
                    Content = cancellationReason.Content
                };

                return Ok(ApiResult<CancellationReasonResponse>.Succeed(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }
        #endregion
        #region api update
        [HttpPut("CancellationReasons/{id}")]
        [SwaggerOperation(
        Summary = "Update cancellation reason",
        Description = "Updates an existing cancellation reason by its ID."
    )]
        [SwaggerResponse(200, "Cancellation reason updated successfully", typeof(ApiResult<CancellationReasonResponse>))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Cancellation reason not found")]
        [SwaggerResponse(500, "An error occurred while updating the cancellation reason")]
        public async Task<IActionResult> UpdateCancellationReason(int id, [FromBody] UpdateCancellationReasonRequest request)
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
                var cancellationReason = await _service.GetCancellationReasonByIdAsync(id);
                if (cancellationReason == null)
                {
                    return NotFound(ApiResult<string>.Error($"Cancellation reason with ID {id} not found."));
                }

                cancellationReason.Content = request.Content;
                await _service.UpdateCancellationReasonAsync(cancellationReason);

                var response = new CancellationReasonResponse
                {
                    ReasonId = cancellationReason.Id,
                    Content = cancellationReason.Content
                };

                return Ok(ApiResult<CancellationReasonResponse>.Succeed(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }
        #endregion
        #region api delete
        [HttpDelete("CancellationReasons/{id}")]
        [SwaggerOperation(
      Summary = "Delete cancellation reason",
      Description = "Deletes an existing cancellation reason by its ID."
  )]
        [SwaggerResponse(200, "Cancellation reason deleted successfully")]
        [SwaggerResponse(404, "Cancellation reason not found")]
        [SwaggerResponse(500, "An error occurred while deleting the cancellation reason")]
        public async Task<IActionResult> DeleteCancellationReason(int id)
        {
            try
            {
                var cancellationReason = await _service.GetCancellationReasonByIdAsync(id);
                if (cancellationReason == null)
                {
                    return NotFound(ApiResult<string>.Error($"Cancellation reason with ID {id} not found."));
                }

                await _service.DeleteCancellationReasonAsync(id);

                return Ok(ApiResult<string>.Succeed("Cancellation reason deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }
        #endregion
    }
}
