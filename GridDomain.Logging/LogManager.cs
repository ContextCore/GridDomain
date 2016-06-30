
namespace GridDomain.Logging
{
    public static class LogManager
    {
        private static LoggerFactory _loggerFactory = new DefaultLoggerFactory();

        public static void SetLoggerFactory(LoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public static ISoloLogger GetLogger()
        {
            return _loggerFactory.GetLogger();
        }
    }
}
