using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExcelTool.Infrastructures.JTW
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class TokenRespondDto
    {
        public string Token { get; set; }
    }

    //认证服务接口
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginDto request, out string token);
    }

    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserService _userService;
        private readonly JwtSetting _jwtSetting;

        public TokenAuthenticationService(IUserService userService, IOptions<AppConfig> AppConfig)
        {
            _userService = userService;
            _jwtSetting = AppConfig.Value.JwtSetting;
        }

        public bool IsAuthenticated(LoginDto request, out string token)
        {
            token = string.Empty;
            if (!_userService.IsValid(request))
                return false;
            var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(_jwtSetting.Issuer, _jwtSetting.Audience, claims, expires: DateTime.Now.AddMinutes(_jwtSetting.AccessExpiration), signingCredentials: credentials);
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return true;
        }
    }
}
