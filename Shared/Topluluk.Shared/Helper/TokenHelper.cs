using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Topluluk.Shared.Dtos;

namespace Topluluk.Shared.Helper
{
	public class TokenHelper
	{
		private readonly IConfiguration _configuration;

		public TokenHelper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

        public static string GetUserNameByToken(HttpRequest request)
        {
            if (request == null || request.Headers == null || !request.Headers.ContainsKey("Authorization") || request.Headers["Authorization"].Count == 0)
            {
                return string.Empty;
            }
            var token = request.Headers["Authorization"][0];
            token = token.Split("Bearer ")[1];
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var username = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            return username ?? throw new Exception($"{typeof(TokenHelper).Name}:Username not found in token");
        }
        public static string GetUserIdByToken(HttpRequest request)
        {
            if (request == null || request.Headers == null || !request.Headers.ContainsKey("Authorization") || request.Headers["Authorization"].Count == 0)
            {
                return string.Empty;
            }
            var token = request.Headers["Authorization"][0];
            token = token.Split("Bearer ")[1];
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var userId = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId ?? throw new Exception($"{typeof(TokenHelper).Name}:UserId not found in token");
        }
        
        public static List<string> GetUserRolesByToken(HttpRequest request)
        {
            if (request == null || request.Headers == null || !request.Headers.ContainsKey("Authorization") || request.Headers["Authorization"].Count == 0)
            {
                return new();
            }
            var token = request.Headers["Authorization"][0];
            token = token.Split("Bearer ")[1];
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var userRoles = jwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.Role)?.Select(c => c.Value).ToList();

            return userRoles ?? throw new Exception($"{typeof(TokenHelper).Name}:UserId not found in token");
        }
        public static string GetToken(HttpRequest request)
        {
            if (request == null || request.Headers == null || !request.Headers.ContainsKey("Authorization") || request.Headers["Authorization"].Count == 0)
            {
                return string.Empty;
            }
            var token = request.Headers["Authorization"][0];
            return token;
        }
        public static bool GetByTokenControl(HttpRequest request)
        {
            if (request != null && request.Headers != null && request.Headers["Authorization"].Count == 0)
            {
                return false;
            }
            else
                return true;
        }

        public TokenDto CreateAccessToken( string userId, string userName, List<string> roles ,int month)
		{
			TokenDto token = new();
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // Şifrlenmiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            var authClaims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Name, userName),
            };
            foreach (var userRole in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            // Oluşturulacak token ayarlarını veriyoruz.
            token.ExpiredAt = DateTime.UtcNow.AddMonths(month);

            JwtSecurityToken securityToken = new(
                audience: _configuration["JWT:ValidAudience"],
                issuer: _configuration["JWT:ValidIssuer"],
                expires: token.ExpiredAt,
                notBefore: DateTime.UtcNow, // Bu token üretildiği anda devreye girecek
                signingCredentials: signingCredentials, // Security key buradaki bilgiler doğrultusunda olacak.
                claims: authClaims
            );

            // Token oluşturucu sınıfından bir örnek alalım.
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);

            token.RefreshToken = CreateRefreshToken();

            return token;
        }

        public  string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using RandomNumberGenerator random = RandomNumberGenerator.Create(); // Using yazmamızın sebebi RnadomNumberGenerator nesnemiz IDisposable dan miras almış olması
            random.GetBytes(number);

            return Convert.ToBase64String(number);
        }



	}
}

