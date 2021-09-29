namespace ChatCase.Business.Services.Logging
{
    public class LoggerFactory
    {
        private LoggerFactory()
        {
        }

        static readonly object _lock = new();
        private static DatabaseLogger _databaseLogger;
        private static FileLogger _fileLogger;

        public static DatabaseLogger DatabaseLogManager()
        {
            if (_databaseLogger == null)
            {
                lock (_lock)
                {
                    if (_databaseLogger == null)
                        _databaseLogger = new DatabaseLogger();
                }
            }
            return _databaseLogger;
        }

        public static FileLogger FileLogManager()
        {
            if (_fileLogger == null)
            {
                lock (_lock)
                {
                    if (_fileLogger == null)
                        _fileLogger = new FileLogger();
                }
            }
            return _fileLogger;
        }
    }
}
