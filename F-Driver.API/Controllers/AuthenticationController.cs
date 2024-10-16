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
    [Route("api/[controller]")]
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
        #region api login for driver
        [AllowAnonymous]
        [HttpPost("login-driver")]
        public async Task<IActionResult> LoginDriver([FromBody] LoginRequest request)
        {
            try
            {
                var res =  _identityService.LoginDriver(request.Email, request.Password);

                if (!res.Authenticated)
                {                  
                    var resultFail = new LoginResponse
                    {
                        AccessToken = null,
                        RefreshToken = null
                    };                   
                    return BadRequest(ApiResult<LoginResponse>.Fail(new Exception("Login failed. Please check your credentials or verify if the account is a driver.")));
                }
             
                var handler = new JwtSecurityTokenHandler();
                var result = new LoginResponse
                {
                    AccessToken = handler.WriteToken(res.Token),
                    RefreshToken = handler.WriteToken(res.RefreshToken)
                };

                return Ok(ApiResult<LoginResponse>.Succeed(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }

        #endregion

        [AllowAnonymous]
        [HttpPost("google-login-passenger")]
        public async Task<IActionResult> GoogleLoginPassenger([FromBody] LoginGoogleRequest request)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var res = await _identityService.LoginGooglePassenger(request.IdToken);

                if (!res.Authenticated)
                {
                    var resultFail = new LoginResponse
                    {
                        AccessToken = null,
                        RefreshToken = null
                    };

                    return BadRequest(ApiResult<LoginResponse>.Fail(new Exception("Login failed. Please check your credentials.")));
                }

                
                var result = new LoginResponse
                {
                    AccessToken = handler.WriteToken(res.Token),
                    RefreshToken = handler.WriteToken(res.RefreshToken)
                };

                return Ok(ApiResult<LoginResponse>.Succeed(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }
        #region api get information
        [Authorize]
        [HttpGet("profile")]
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
