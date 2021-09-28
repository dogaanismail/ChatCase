using ChatCase.Business.Configuration.Common;
using ChatCase.Core.Attributes;
using ChatCase.Core.Configuration.Configs;
using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Infrastructure;
using ChatCase.Core.Security.JwtSecurity;
using ChatCase.Domain.Dto.Request.Identity;
using ChatCase.Domain.Enumerations;
using ChatCase.Domain.Validation.Identity;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ChatCase.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        /// <param name="webHostEnvironment">Hosting environment</param>
        /// <returns>Configured service provider</returns>
        public static (IEngine, AppConfig) ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            CommonHelper.DefaultFileProvider = new SystemFileProvider(webHostEnvironment);

            services.AddHttpContextAccessor();

            var appConfigs = new AppConfig();
            configuration.Bind(appConfigs);
            services.AddSingleton(appConfigs);
            AppConfigsHelper.SaveAppSettings(appConfigs);

            var engine = EngineContext.Create();

            engine.ConfigureServices(services, configuration);
            engine.RegisterDependencies(services, appConfigs);

            return (engine, appConfigs);
        }

        /// <summary>
        /// Register httpContextAccessor
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Register mvc configurations
        /// </summary>
        /// <param name="services"></param>
        public static void AddSystemMvc(this IServiceCollection services)
        {
            services.AddResponseCompression(
             options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                             {
                                    "image/jpeg",
                                    "image/png",
                                    "image/gif"
                             }));

            services.AddCors();

            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
                opt.Filters.Add(typeof(ValidateModelAttribute));
            }).SetCompatibilityVersion(CompatibilityVersion.Latest)
            .AddFluentValidation(fvc => { });
        }

        /// <summary>
        /// Register swagger implementation
        /// </summary>
        /// <param name="services"></param>
        public static void AddSystemSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatCase.Api", Version = "1.0.0" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddSystemAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = Singleton<AppConfig>.Instance;
            var mongoDbSettings = appSettings.MongoDbConfig;
            var jwtConfigs = appSettings.JwtConfig;

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddMongoDbStores<AppUser, AppRole, string>
            (
                mongoDbSettings.ConnectionString, mongoDbSettings.Database
            );

            services.AddAuthentication().AddCookie(options =>
            {
                options.Cookie.Name = "Interop";
                options.DataProtectionProvider =
                    DataProtectionProvider.Create(new DirectoryInfo("C:\\Github\\Identity\\artifacts"));
            });

            JwtTokenDefinitions.LoadFromConfiguration(jwtConfigs);
            services.ConfigureJwtAuthentication();
            services.ConfigureJwtAuthorization();
        }

        /// <summary>
        /// Register services required for distributed cache
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddSystemDistributedCache(this IServiceCollection services)
        {
            var appSettings = Singleton<AppConfig>.Instance;
            var distributedCacheConfig = appSettings.DistributedCacheConfig;

            if (!distributedCacheConfig.Enabled)
                return;

            switch (distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.Memory:
                    services.AddDistributedMemoryCache();
                    break;

                case DistributedCacheType.SqlServer:
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = distributedCacheConfig.ConnectionString;
                        options.SchemaName = distributedCacheConfig.SchemaName;
                        options.TableName = distributedCacheConfig.TableName;
                    });
                    break;

                case DistributedCacheType.Redis:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                    });
                    break;
            }
        }

        /// <summary>
        /// Register behavior options
        /// </summary>
        /// <param name="services"></param>
        public static void AddBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });
        }

        /// <summary>
        /// Register the validators
        /// </summary>
        /// <param name="services"></param>
        public static void AddSystemValidator(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddSingleton<IValidator<RegisterRequest>, RegisterRequestValidator>();
        }
    }
}
