using AutoMapper;
using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;

namespace F_Driver.API.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;
        private readonly IMapper _mapper;

        public FeedbackController(FeedbackService feedbackService, IMapper mapper)
        {
            _feedbackService = feedbackService;
            _mapper = mapper;
        }

        [HttpPost("")]
        [SwaggerOperation(
    Summary = "Create feedback",
    Description = "Allows a user to provide feedback on a completed trip."
)]
        [SwaggerResponse(200, "Feedback created successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Trip not found or invalid feedback details")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An error occurred while creating feedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackRequest request)
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

                // Map FeedbackRequest to FeedbackCreateModel
                var feedbackCreateModel = request.MapToModel();

                // Set the passengerId in the feedback model
                feedbackCreateModel.PassengerId = passengerId;

                // Call the service to create feedback
                await _feedbackService.CreateFeedbackAsync(feedbackCreateModel);

                return Ok(ApiResult<string>.Succeed("Feedback created successfully."));
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
    }
}
