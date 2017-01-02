namespace GridDomain.Logging
{
    public static class LogManager
    {
        private static LoggerFactory _loggerFactory = new DefaultLoggerFactory();
        public static void SetLoggerFactory(LoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public static ILogger GetLogger(LoggerFactory factory = null)
        {
            return factory != null ? factory.GetLogger() : _loggerFactory.GetLogger();
        }
    }
}
