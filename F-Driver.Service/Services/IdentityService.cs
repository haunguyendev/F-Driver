using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Helpers;
using F_Driver.Repository.Interfaces;
using F_Driver.Repository.Repositories;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Settings;
using F_Driver.Service.Shared;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pkix;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class IdentityService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public IdentityService( IMapper mapper, IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtSettingsOptions)
        {
            _jwtSettings = jwtSettingsOptions.Value;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

       
        #region login for driver
        public LoginResult LoginDriver(string email, string password)
        {
            // Tìm người dùng bằng email
            var user = _unitOfWork.Users.FindByCondition(c => c.Email == email).FirstOrDefault();

            if (user == null)
            {
                // Throw exception khi không tìm thấy người dùng
                throw new Exception("Email not found.");
            }

            // Kiểm tra xem người dùng có phải là tài xế không
            if (!user.Role.Equals("Driver", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("User is not a driver.");
            }

            // Kiểm tra mật khẩu
            if (!user.PasswordHash.Equals(password))
            {
                throw new Exception("Incorrect password.");
            }

            // Đăng nhập thành công, trả về token
            return new LoginResult
            {
                Authenticated = true,
                Token = CreateJwtToken(user),
                RefreshToken = CreateJwtRefreshToken(user)
            };
        }


        #endregion
        #region login for admin
        public LoginResult LoginAdmin(string email, string password)
        {
            // Tìm người dùng bằng email
            var user = _unitOfWork.Admins.FindByCondition(c => c.Email == email).FirstOrDefault();

            if (user == null)
            {
                throw new Exception("Email not found.");
            }
            if (!user.Password.Equals(password))
            {
                throw new Exception("Incorrect password.");
            }

            return new LoginResult
            {
                Authenticated = true,
                Token = CreateJwtTokenAdmin(user),
                RefreshToken = CreateJwtRefreshTokenAdmin(user)
            };
        }
        #endregion
        #region login google
        public async Task<LoginResult> LoginGooglePassenger(string token)
        {
            try
            {
                var credential = GoogleCredential.FromAccessToken(token);

                var oauth2Service = new Oauth2Service(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "project-289984090391",
                });

                // Lấy thông tin người dùng từ Google
                Userinfo userInfo = await oauth2Service.Userinfo.Get().ExecuteAsync();

                // Kiểm tra xem người dùng với role là 'passenger' đã tồn tại chưa
                var user = await _unitOfWork.Users.FindByCondition(u => u.Email == userInfo.Email && u.Role == UserRoleEnum.PASSENGER).FirstOrDefaultAsync();

                // Nếu không tìm thấy, tạo người dùng mới
                if (user == null)
                {
                    user = new User()
                    {
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                        ProfileImageUrl = userInfo.Picture,
                        Role = UserRoleEnum.PASSENGER,
                        Verified = false,
                        IsMailValid = true,
                        VerificationStatus = UserVerificationStatusEnum.INIT,
                        CreatedAt = DateTime.Now
                    };

                    // Tạo ví mới với số dư là 0
                    var wallet = new Wallet
                    {
                        User = user,
                        Balance = 0, // Khởi tạo số dư là 0
                        CreatedAt = DateTime.Now
                    };

                    // Lưu người dùng và ví vào cơ sở dữ liệu
                    await _unitOfWork.Users.CreateAsync(user);
                    await _unitOfWork.Wallets.CreateAsync(wallet); // Thêm dòng này để lưu ví
                    await _unitOfWork.CommitAsync();
                }
                

                // Tạo JWT Token và Refresh Token cho người dùng
                var tokenResponse = CreateJwtToken(user);
                var tokenRefreshResponse = CreateJwtRefreshToken(user);

                return new LoginResult
                {
                    Authenticated = true,
                    Token = tokenResponse,
                    RefreshToken = tokenRefreshResponse,
                    
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Google login failed.", ex);
            }
        }


        #endregion

        #region Create access token and refresh token for passengers
        public SecurityToken CreateJwtToken(User user)
        {
            var utcNow = DateTime.UtcNow;
            var authClaims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(ClaimTypes.Role, user.Role), //
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = utcNow.Add(TimeSpan.FromHours(1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        private SecurityToken CreateJwtRefreshToken(User user)
        {
            var utcNow = DateTime.UtcNow;
            var authClaims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(ClaimTypes.Role, user.Role),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = utcNow.Add(TimeSpan.FromDays(5)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        #endregion

        #region Create access token and refresh token for admin
        public SecurityToken CreateJwtTokenAdmin(Admin user)
        {
            var utcNow = DateTime.UtcNow;
            var authClaims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(ClaimTypes.Role, "Admin"), //
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = utcNow.Add(TimeSpan.FromHours(1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        private SecurityToken CreateJwtRefreshTokenAdmin(Admin user)
        {
            var utcNow = DateTime.UtcNow;
            var authClaims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(ClaimTypes.Role, "Admin"),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = utcNow.Add(TimeSpan.FromDays(5)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        #endregion

    }
}
