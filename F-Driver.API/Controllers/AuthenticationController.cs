using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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


    }
}
