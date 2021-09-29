using AutoMapper;
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
using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Events;
using ChatCase.Core.Infrastructure;
using ChatCase.Core.Infrastructure.Mapper;
using ChatCase.Framework;
using ChatCase.Repository.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Moq;
using System;
using System.IO;
using System.Linq;

namespace ChatCase.Tests
{
    /// <summary>
    /// Base test abstract class implementations
    /// </summary>
    public abstract class BaseTest
    {
        private static readonly ServiceProvider _serviceProvider;

        static BaseTest()
        {
            var services = new ServiceCollection();

            services.AddHttpClient();

            var typeFinder = new AppDomainTypeFinder();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            var appSettings = new AppConfig();
            services.AddSingleton(appSettings);
            Singleton<AppConfig>.Instance = appSettings;

            var hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
            services.AddSingleton(hostApplicationLifetime.Object);

            var rootPath =
             new DirectoryInfo(
                     $@"{Directory.GetCurrentDirectory().Split("bin")[0]}{Path.Combine(@"\..\ChatCase.Api".Split('\\', '/').ToArray())}")
                 .FullName;

            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(p => p.WebRootPath).Returns(Path.Combine(rootPath, "wwwroot"));
            webHostEnvironment.Setup(p => p.ContentRootPath).Returns(rootPath);
            webHostEnvironment.Setup(p => p.EnvironmentName).Returns("test");
            webHostEnvironment.Setup(p => p.ApplicationName).Returns("chatCase");
            services.AddSingleton(webHostEnvironment.Object);

            CommonHelper.DefaultFileProvider = new SystemFileProvider(webHostEnvironment.Object);
            var httpContext = new DefaultHttpContext
            {
                Request = { Headers = { { HeaderNames.Host, TestsDefaults.HostIpAddress } } }
            };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(p => p.HttpContext).Returns(httpContext);

            services.AddSingleton(httpContextAccessor.Object);

            var actionContextAccessor = new Mock<IActionContextAccessor>();
            actionContextAccessor.Setup(x => x.ActionContext)
                .Returns(new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor()));

            services.AddSingleton(actionContextAccessor.Object);

            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            var urlHelper = new TestUrlHelper(actionContextAccessor.Object.ActionContext);

            urlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper);

            services.AddTransient(provider => actionContextAccessor.Object);

            services.AddSingleton(urlHelperFactory.Object);

            services.AddSingleton<ITypeFinder>(typeFinder);

            services.AddTransient<ISystemFileProvider, SystemFileProvider>();

            services.AddSingleton<IMemoryCache>(memoryCache);
            services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            services.AddSingleton<ILocker, MemoryCacheManager>();

            services.AddTransient(typeof(IRepository<>), typeof(RepositoryBase<>));

            services.AddTransient<IWebHelper, WebHelper>();
            services.AddTransient<IWorkContext, WebWorkContext>();

            #region Services Dependency Registrations

            services.AddTransient<ISettingService, SettingService>();
            services.AddTransient<IChattingService, ChattingService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAppUserActivityService, AppUserActivityService>();
            #endregion

            #region Consumer&Event Dependency Registrations

            services.AddSingleton<IEventPublisher, EventPublisher>();

            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
                foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>)))
                    services.AddTransient(findInterface, consumer);

            #endregion

            #region Setting Dependency Registrations

            var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
            foreach (var setting in settings)
                services.AddTransient(setting,
                    context => context.GetRequiredService<ISettingService>().LoadSettingAsync(setting).Result);

            #endregion

            #region Mapper Configurations

            var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();
            var instances = mapperConfigurations
                .Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            AutoMapperConfiguration.Init(config);

            #endregion

            #region MongoDb Identity Registrations

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
                appSettings.MongoDbConfig.ConnectionString, appSettings.MongoDbConfig.Database
            );

            #endregion

            _serviceProvider = services.BuildServiceProvider();

            EngineContext.Replace(new DevTestEngine(_serviceProvider));
        }

        #region Utilities

        public T GetService<T>()
        {
            try
            {
                return _serviceProvider.GetRequiredService<T>();
            }
            catch (InvalidOperationException ex)
            {
                return (T)EngineContext.Current.ResolveUnregistered(typeof(T));
            }
        }

        public partial class DevTestEngine : SystemEngine
        {
            protected readonly IServiceProvider _internalServiceProvider;

            public DevTestEngine(IServiceProvider serviceProvider)
            {
                _internalServiceProvider = serviceProvider;
            }

            public override IServiceProvider ServiceProvider => _internalServiceProvider;
        }

        protected class TestUrlHelper : UrlHelperBase
        {
            public TestUrlHelper(ActionContext actionContext) : base(actionContext)
            {
            }

            public override string Action(UrlActionContext actionContext)
            {
                return string.Empty;
            }

            public override string RouteUrl(UrlRouteContext routeContext)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
