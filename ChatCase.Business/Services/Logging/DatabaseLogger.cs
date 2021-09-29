using ChatCase.Business.Interfaces.Logging;
using ChatCase.Core.Configuration.Configs;
using ChatCase.Core.Infrastructure;
using Serilog;
using Serilog.Core;

namespace ChatCase.Business.Services.Logging
{
    public class DatabaseLogger : LoggingConfiguration, ILogService
    {
        protected override Logger GetLogger()
        {
            var _appConfig = EngineContext.Current.Resolve<AppConfig>();

            return new LoggerConfiguration()
                    .WriteTo.MongoDB(_appConfig.MongoDbConfig.ConnectionString, collectionName: "applog")
                    .CreateLogger();
        }

        public void Verbose(string message) => GetLogger().Verbose(message);
        public void Verbose<T>(string message, T t) => GetLogger().Verbose(message, t);
        public void Fatal(string message) => GetLogger().Fatal(message);
        public void Fatal<T>(string message, T t) => GetLogger().Fatal(message, t);
        public void Information(string message) => GetLogger().Information(message);
        public void Information<T>(string message, T t) => GetLogger().Information(message, t);
        public void Warning(string message) => GetLogger().Warning(message);
        public void Warning<T>(string message, T t) => GetLogger().Warning(message, t);
        public void Debug(string message) => GetLogger().Debug(message);
        public void Debug<T>(string message, T t) => GetLogger().Debug(message, t);
        public void Error(string message) => GetLogger().Error(message);
        public void Error<T>(string message, T t) => GetLogger().Error(message, t);
    }
}
