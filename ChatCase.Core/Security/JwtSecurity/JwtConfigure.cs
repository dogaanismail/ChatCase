﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
                auth.AddPolicy("BearerNegotiateNTLM ", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        RequireExpirationTime = true,
                        IssuerSigningKey = JwtTokenDefinitions.IssuerSigningKey,
                        ValidAudience = JwtTokenDefinitions.Audience,
                        ValidIssuer = JwtTokenDefinitions.Issuer,
                        ValidateIssuerSigningKey = JwtTokenDefinitions.ValidateIssuerSigningKey,
                        ValidateLifetime = JwtTokenDefinitions.ValidateLifetime,
                        ClockSkew = JwtTokenDefinitions.ClockSkew
                    };
                });
        }

        #endregion
    }
}