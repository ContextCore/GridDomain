
namespace GridDomain.Logging
{
    public static class LogManager
    {
        private static ILoggerFactory _loggerFactory = new DefaultLoggerFactory();

        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
         _loggerFactory = loggerFactory;
        }

        public static ISoloLogger GetLogger()
        {
            return _loggerFactory.GetLogger();
        }
    }
}
