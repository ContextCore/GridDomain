using System;
using NLog;

namespace GridDomain.Scheduling.Integration
{
    public class DefaultSoloLogger : ISoloLogger
    {
        private readonly ILogger _logger = NLog.LogManager.GetLogger("default");
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Error(Exception error, string message)
        {
            _logger.Error(error, message);
        }
    }
}