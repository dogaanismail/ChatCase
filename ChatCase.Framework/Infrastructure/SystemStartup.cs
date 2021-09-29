using ChatCase.Core.Infrastructure;
using ChatCase.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatCase.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring MVC on application startup
    /// </summary>
    public class SystemStartup : IStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSystemMvc();

            services.AddSystemAuthentication(configuration);

            services.AddOptions();

            services.AddSystemValidator();

            services.AddSystemSwagger();

            services.AddBehaviorOptions();

            services.AddSystemDistributedCache();

            services.AddSignalR();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.UseAuthentication();

            application.UseAuthorization();

            application.UseSystemEnvironment();

            application.UseSystemStaticFiles();

            application.UseSystemRouting();

            application.UseSystemSwagger();

            application.UseSystemMvc();
           
            application.UseSystemEndPoint();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1000; //MVC should be loaded last
    }
}
