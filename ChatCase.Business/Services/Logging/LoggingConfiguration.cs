using Serilog.Core;

namespace ChatCase.Business.Services.Logging
{
    public abstract class LoggingConfiguration
    {
        protected abstract Logger GetLogger();

        public Logger InstanceLogger()
        {
            return default;
        }
    }
}
