using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Helpers;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace F_Driver.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;

        public UserController(UserService userService, IMemoryCache cache, EmailService emailService)
        {
            _userService = userService;
            _cache = cache;
            _emailService = emailService;
        }


        #region func for send mail
        private async Task SendOtpAsync(string email, string subject)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set(email, otp, TimeSpan.FromMinutes(10));
            var mailData = new MailData
            {
                EmailToId = email,
                EmailToName = email,
                EmailSubject = subject,
                EmailBody = $"Your OTP is: {otp}"
            };

            await _emailService.SendEmailAsync(mailData);
        }
        #endregion
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromForm] UserRequestModel userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var check = userRequest.Verified;
                if (userRequest.Verified == true && !userRequest.VerificationStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Verification status must be 'Spending' if user is verified" });
                }
                if (userRequest.IsMailValid == true)
                {
                    return BadRequest(new { message = "Mail validation must be verified by admin" });
                }
                if (userRequest.Driver.Verified == true)
                {
                    return BadRequest(new { message = "Driver must be verified by admin" });
                }
                if (userRequest.Driver.Vehicles == null || !userRequest.Driver.Vehicles.Any(v => v.IsVerified))
                {
                    return BadRequest(new { message = "Driver must have at least one vehicle" });
                }

                if (userRequest.Driver.Vehicles.Any(v => v.IsVerified))
                {
                    return BadRequest(new { message = "Vehicles must be approved by admin before use." });
                }
                var createdUser = await _userService.CreateUserAsync(userRequest.MapToUserModel());
                return Ok(new { message = "User created successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("otp/send")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (!regex.IsMatch(request.Email))
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Invalid email format."));
                return BadRequest(result);
            }
            var user = await _userService.GetUserById(request.UserId);
            if (user != null)
            {
                if(user.Email != request.Email)
                {
                    var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Email does not match with the user."));
                    return BadRequest(result);
                }
                if (request.IsResend)
                {
                    if (!_cache.TryGetValue(request.Email, out string _))
                    {
                        var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Email not found. Please initiate the forget password process first."));
                        return NotFound(result);
                    }
                    _cache.Remove(request.Email);
                }

                await SendOtpAsync(request.Email, request.IsResend ? "Resend OTP" : "Reset Password OTP");

                var response = ApiResult<SendOtpResponse>.Succeed(new SendOtpResponse { Message = "OTP sent successfully." });
                return Ok(response);
            }
            return NotFound(ApiResult<Dictionary<string, string[]>>.Fail(new Exception("User is not found")));

        }

        [HttpPost("otp/verify")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (!regex.IsMatch(request.Email))
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Invalid email format."));
                return BadRequest(result);
            }
            if (_cache.TryGetValue(request.Email, out string otp) && otp == request.Otp)
            {
                _cache.Remove(request.Email);
                var response = ApiResult<SendOtpResponse>.Succeed(new SendOtpResponse { Message = "OTP verified. You can now reset your password." });
                return Ok(response);
            }
            return Unauthorized(ApiResult<SendOtpResponse>.Succeed(new SendOtpResponse { Message = "Invalid OTP" }));
        }

        [HttpPost("change-status")]
        public async Task<IActionResult> HandleStatusCodes([FromBody] ErrorRequest request)
        {
            try
            {
                var user = _userService.GetUserById(request.UserId).Result;
                if (user == null)
                {
                    return NotFound(ApiResult<Dictionary<string, string[]>>.Fail(new Exception("User not found")));
                }
                // Call the service to handle error codes
                var errorDetails = await _userService.HandleStatusVerifyCodes(request.ErrorCodes, request.UserId);

                // Return the response with user ID and error details
                return Ok(ApiResult<StatusUserResponse>.Succeed(new StatusUserResponse { Message = errorDetails}));
            }
            catch (InvalidOperationException ex)
            {
                // If there's an invalid combination of error codes, return a bad request
                return BadRequest(ApiResult<Dictionary<string, string[]>>.Fail(new Exception(ex.Message)));
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryParameters parameters)
        {
            try
            {
                // Gọi phương thức từ service để lấy danh sách người dùng theo bộ lọc
                var users = await _userService.GetUsersAsync(parameters);

                // Chuẩn bị đối tượng trả về theo kiểu phân trang
                var paginatedUsers = new PaginatedResult<UserModel>
                {
                    Page = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalItems = users.TotalCount,
                    TotalPages = (int)Math.Ceiling(users.TotalCount / (double)parameters.PageSize),
                    Data = users.Items
                };

                // Trả về kết quả thành công
                return Ok(ApiResult<PaginatedResult<UserModel>>.Succeed(paginatedUsers));
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return BadRequest(ApiResult<string>.Fail(ex));
            }
        }

    }
}
