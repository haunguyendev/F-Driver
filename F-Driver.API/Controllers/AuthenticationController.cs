using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Services;
using F_Driver.Service.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace F_Driver.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IdentityService _identityService;
        private readonly UserService _userService;
        public AuthenticationController(IdentityService identityService,UserService userService)
        {
            _identityService = identityService;
            _userService = userService; 
            
        }
        #region 
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
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
            var handler = new JwtSecurityTokenHandler();
            try
            {
                if (loginRequest.Role.ToLower() == "passenger")
                {
                    if (string.IsNullOrEmpty(loginRequest.IdToken))
                    {
                        return BadRequest("ID Token is required for Passenger login.");
                    }

                    var res = await _identityService.LoginGooglePassenger(loginRequest.IdToken);
                    var result = new LoginResponse
                    {
                        AccessToken = handler.WriteToken(res.Token),
                        RefreshToken = handler.WriteToken(res.RefreshToken)
                    };

                    return Ok(ApiResult<LoginResponse>.Succeed(result));
                }
                else if (loginRequest.Role.ToLower() == "driver")
                {
                    if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                    {
                        return BadRequest("Email and Password are required for Driver login.");
                    }

                    var res = _identityService.LoginDriver(loginRequest.Email, loginRequest.Password);
                    var result = new LoginResponse
                    {
                        AccessToken = handler.WriteToken(res.Token),
                        RefreshToken = handler.WriteToken(res.RefreshToken)
                    };

                    return Ok(ApiResult<LoginResponse>.Succeed(result));
                }
                else if (loginRequest.Role.ToLower() == "admin")
                {
                    if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                    {
                        return BadRequest("Email and Password are required for Admin login.");
                    }

                    var res = _identityService.LoginAdmin(loginRequest.Email, loginRequest.Password);
                    var result = new LoginResponse
                    {
                        AccessToken = handler.WriteToken(res.Token),
                        RefreshToken = handler.WriteToken(res.RefreshToken)
                    };

                    return Ok(ApiResult<LoginResponse>.Succeed(result));
                }
                else
                {
                    return BadRequest("Invalid role.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<string>.Fail(ex));
            }

        }
            #endregion
        #region api get information
        [Authorize]
        [HttpGet("users/profile")]
        [SwaggerOperation(
    Summary = "Get user profile",
    Description = "Returns user profile details based on the user's role (Passenger or Driver)."
)]
        [SwaggerResponse(StatusCodes.Status200OK, "User profile retrieved successfully", typeof(ApiResult<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult<object>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the user profile", typeof(ApiResult<object>))]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userRole))
                {
                    return BadRequest(ApiResult<string>.Error("Role not found in token"));
                }

                if (userRole == UserRoleEnum.PASSENGER)
                {
                    var passenger = await _userService.GetPassengerDetailById(userId);
                    if (passenger == null)
                    {
                        return NotFound(ApiResult<string>.Error("Passenger not found"));
                    }

                    return Ok(ApiResult<object>.Succeed(passenger));
                }
                else if (userRole == UserRoleEnum.DRIVER)
                {
                    var driver = await _userService.GetDriverDetailById(userId);
                    if (driver == null)
                    {
                        return NotFound(ApiResult<string>.Error("Driver not found"));
                    }

                   

                    return Ok(ApiResult<object>.Succeed(driver));
                }
                else
                {
                    return BadRequest(ApiResult<string>.Error("Unsupported role"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
            
        }

        #endregion



    }
}
