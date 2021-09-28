using ChatCase.Core.Security.JwtSecurity;
using ChatCase.Domain.Dto.Response.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ChatCase.Core.Security
{
    /// <summary>
    /// Token service implementation
    /// </summary>
    public class TokenService : ITokenService
    {
        #region Methods

        /// <summary>
        /// Generates JWT token
        /// </summary>
        /// <param name="appUserDto"></param>
        /// <returns></returns>
        public TokenUserResponse GenerateToken(AppUserDto appUserDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUserDto.UserId.ToString())
            };

            string accessToken = GenerateAccessToken(claims);
            string refreshToken = GenerateRefreshToken();
            var token = new TokenUserResponse
            {
                UserId = appUserDto.UserId,
                UserName = appUserDto.UserName,
                RegisteredDate = appUserDto.RegisteredDate,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expires = JwtTokenDefinitions.TokenExpirationTime.Ticks
            };

            return token;
        }

        /// <summary>
        /// Generates access token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var handler = new JwtSecurityTokenHandler();
            var securityTokenHanlder = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = JwtTokenDefinitions.Issuer,
                Audience = JwtTokenDefinitions.Audience,
                SigningCredentials = JwtTokenDefinitions.SigningCredentials,
                Expires = DateTime.Now.Add(JwtTokenDefinitions.TokenExpirationTime),
                NotBefore = DateTime.Now
            });
            string accessToken = handler.WriteToken(securityTokenHanlder);

            return accessToken;
        }

        /// <summary>
        /// Generates refresh token
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        #endregion
    }
}
