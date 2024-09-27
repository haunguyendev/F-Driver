using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Settings;
using F_Driver.Service.Shared;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public IdentityService( IMapper mapper, IUnitOfWork unitOfWork, JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #region login google
        public async Task<LoginResult> LoginGooglePassenger(string token)
        {
            try
            {
                var credential = GoogleCredential.FromAccessToken(token);
                var oauth2Service = new Oauth2Service(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "F-Driver",
                });

                // Lấy thông tin người dùng từ Google
                Userinfo userInfo = await oauth2Service.Userinfo.Get().ExecuteAsync();

                // Kiểm tra xem người dùng với role là 'passenger' đã tồn tại chưa
                var user = await _unitOfWork.Users.FindByCondition(u => u.Email == userInfo.Email && u.Role ==UserRoleEnum.PASSENGER).FirstOrDefaultAsync();

                // Nếu không tìm thấy, tạo người dùng mới
                if (user == null)
                {
                    user = new User()
                    {
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                        ProfileImageUrl = userInfo.Picture,
                        Role = "passenger", // Gán role là passenger
                        Verified = false,
                        VerificationStatus = "Pending",
                        CreatedAt = DateTime.Now
                    };

                    
                    await _unitOfWork.Users.CreateAsync(user);
                    await _unitOfWork.CommitAsync();
                }

                

                // Tạo JWT Token và Refresh Token cho người dùng
                var tokenResponse = CreateJwtTokenPassenger(user);
                var tokenRefreshResponse = CreateJwtRefreshTokenPassenger(user);

                return new LoginResult
                {
                    Authenticated = true,
                    Token = tokenResponse,
                    RefreshToken = tokenRefreshResponse
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Authenticated = false,
                    Token = null,
                    RefreshToken = null
                };
            }
        }

        #endregion

        #region Create access token and refresh token for passengers
        public SecurityToken CreateJwtTokenPassenger(User user)
        {
            var utcNow = DateTime.UtcNow;
            var authClaims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(ClaimTypes.Role, "Passenger"), //
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

        private SecurityToken CreateJwtRefreshTokenPassenger(User user)
        {
            var utcNow = DateTime.UtcNow;
            var authClaims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(ClaimTypes.Role, "Passenger"),
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
