using ChatCase.Domain.Dto.Response.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace ChatCase.Core.Security
{
    /// <summary>
    /// ITokenService interface implementation
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates JWT token
        /// </summary>
        /// <param name="appUserDto"></param>
        /// <returns></returns>
        TokenUserResponse GenerateToken(AppUserDto appUserDto);

        /// <summary>
        /// Generate access token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateAccessToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Generate refresh token
        /// </summary>
        /// <returns></returns>
        string GenerateRefreshToken();
    }
}
