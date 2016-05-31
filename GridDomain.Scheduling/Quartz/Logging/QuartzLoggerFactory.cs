using System;
using NLog;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public static class QuartzLoggerFactory
    {
        private static Func<IQuartzLogger> _loggerFactory;

        public static void SetLoggerFactory(Func<IQuartzLogger> loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public static IQuartzLogger GetLogger()
        {
            if (_loggerFactory == null)
            {
                _loggerFactory = () => new QuartzLogger(LogManager.GetLogger("QuartzLogger"));
            }
            return _loggerFactory();
        }
    }
}