using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;

namespace ChatCase.Core.Security.JwtSecurity
{
    /// <summary>
    /// Jwt configurations
    /// </summary>
    public static class JwtConfigure
    {
        #region Methods
        public static void ConfigureJwtAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationCore(auth =>
            {
                auth.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });
            });
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = JwtTokenDefinitions.Issuer,
                        ValidAudience = JwtTokenDefinitions.Audience,
                        IssuerSigningKey = JwtTokenDefinitions.IssuerSigningKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        #endregion
    }
}
