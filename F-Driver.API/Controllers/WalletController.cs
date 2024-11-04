using F_Driver.API.Common;
using F_Driver.API.Exceptions;
using F_Driver.API.Payloads.Request;
using F_Driver.Service.DTO.VNPay;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using System.IdentityModel.Tokens.Jwt;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }


        //update wallet
        [HttpPut]
        public async Task<IActionResult> UpdateWalletAsync([FromBody] UpdateWalletRequest model)
        {
            try
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
                    throw new BadRequestException("Authorization header is missing or invalid.");
                }

                token = token.ToString().Split()[1];

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new BadRequestException("Authorization header is missing or invalid.");
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var customerClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);

                if (customerClaim == null)
                {
                    return Unauthorized(ApiResult<string>.Error("Unauthorized: No customer ID found in token."));
                }

                var userId = int.Parse(customerClaim.Value);
                if(model.Balance <= 0)
                {
                    return BadRequest("Balance must be greater than 0.");
                }
                var result =  await _walletService.UpdateWalletAsync(userId, model.Balance);
                if (!result.Success)
                {
                    return BadRequest(ApiResult<string>.Error(result.ErrorMessage));
                }

                return Ok(ApiResult<string>.Succeed(result.PaymentUrl));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //update wallet with payment success
        [HttpGet("success")]
        public async Task<IActionResult> UpdatePaymentStatus([FromQuery] UpdateVNPayModel request)
        {
            try
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

                var res = await _walletService.UpdatePaymentStatusAsync(request);

                return Ok(ApiResult<string>.Succeed(res));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

    }
}
