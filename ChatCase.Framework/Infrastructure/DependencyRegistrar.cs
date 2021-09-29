using ChatCase.Business.Events;
using ChatCase.Business.Interfaces.Chatting;
using ChatCase.Business.Interfaces.Configuration;
using ChatCase.Business.Interfaces.Identity;
using ChatCase.Business.Interfaces.Logging;
using ChatCase.Business.Services.Chatting;
using ChatCase.Business.Services.Configuration;
using ChatCase.Business.Services.Identity;
using ChatCase.Business.Services.Logging;
using ChatCase.Core;
using ChatCase.Core.Caching;
using ChatCase.Core.Configuration.Configs;
using ChatCase.Core.Configuration.Settings;
using ChatCase.Core.Events;
using ChatCase.Core.Infrastructure;
using ChatCase.Core.Infrastructure.DependencyManagement;
using ChatCase.Repository.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ChatCase.Framework.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="typeFinder"></param>
        /// <param name="config"></param>
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppConfig appConfig)
        {
            services.AddScoped<ISystemFileProvider, SystemFileProvider>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));

            services.AddScoped<IWorkContext, WebWorkContext>();
            services.AddScoped<IWebHelper, WebHelper>();

            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAppUserActivityService, AppUserActivityService>();
            services.AddScoped<IChattingService, ChattingService>();

            services.AddSingleton<IEventPublisher, EventPublisher>();

            #region Caching implementations

            if (appConfig.DistributedCacheConfig.Enabled)
            {
                services.AddScoped<ILocker, DistributedCacheManager>();
                services.AddScoped<IStaticCacheManager, DistributedCacheManager>();
            }
            else
            {
                services.AddSingleton<ILocker, MemoryCacheManager>();
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            }

            #endregion

            #region Consumer Dependency Registrations

            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
                foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>)))
                    services.AddScoped(findInterface, consumer);

            #endregion

            #region Setting Dependency Registrations

            if (appConfig.CommonConfig.RegisterSettings)
            {
                var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
                foreach (var setting in settings)
                {
                    services.AddScoped(setting, serviceProvider =>
                    {
                        return serviceProvider.GetRequiredService<ISettingService>().LoadSettingAsync(setting).Result;
                    });
                }
            }

            #endregion
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 0;
    }
}
