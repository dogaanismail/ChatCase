using ChatCase.Business.Services.Logging;
using ChatCase.Core.Infrastructure;
using ChatCase.Framework.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net;

namespace ChatCase.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application"></param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }


        /// <summary>
        /// Configure exception handling
        /// </summary>
        /// <param name="application"></param>
        public static void UseSystemExceptionHandler(this IApplicationBuilder application)
        {
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

            application.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return;

                    try
                    {
                        LoggerFactory.DatabaseLogManager().Fatal($"SystemError : {JsonConvert.SerializeObject(exception)}");
                        LoggerFactory.FileLogManager().Fatal($"SystemError : {JsonConvert.SerializeObject(exception)}");
                    }
                    finally
                    {
                        var code = HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)code;
                        await context.Response.WriteAsync(exception.Message);
                    }
                });
            });
        }

        /// <summary>
        /// Configure environment
        /// </summary>
        /// <param name="application"></param>
        public static void UseSystemEnvironment(this IApplicationBuilder application)
        {
            var env = EngineContext.Current.Resolve<IWebHostEnvironment>();

            if (env.IsDevelopment())
                application.UseDeveloperExceptionPage();

            else
                application.UseHsts();
        }

        /// <summary>
        /// Configure static file serving
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseSystemStaticFiles(this IApplicationBuilder application)
        {
            application.UseHttpsRedirection().UseResponseCompression();
        }

        /// <summary>
        /// Configure static file serving
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseSystemMvc(this IApplicationBuilder application)
        {
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

            //using cors
            application.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            application.UseMvc();
        }

        /// <summary>
        /// Configure swagger
        /// </summary>
        /// <param name="application"></param>
        public static void UseSystemSwagger(this IApplicationBuilder application)
        {
            application.UseSwagger();
            application.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatCase.Api V1");
                c.DocumentTitle = "Title";
                c.DisplayOperationId();
                c.DocExpansion(DocExpansion.None);
            });
        }


        /// <summary>
        /// Configure routing
        /// </summary>
        /// <param name="application"></param>
        public static void UseSystemRouting(this IApplicationBuilder application)
        {
            application.UseRouting();
        }

        /// <summary>
        /// Configure systerm end point
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseSystemEndPoint(this IApplicationBuilder application)
        {
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatGroupHub>("/chatHub");

                endpoints.MapControllers();
            });
        }
    }
}
