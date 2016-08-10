using System.Diagnostics;
using Serilog;

namespace GridDomain.Logging
{
    public class DefaultLoggerFactory : ILoggerFactory
    {
        private readonly LoggerConfiguration _configuration;
        private static SerilogLogger _serilogLogger;

        public DefaultLoggerFactory(LoggerConfiguration configuration = null)
        {
            _configuration = configuration ?? new DefaultLoggerConfiguration();
            _serilogLogger = new SerilogLogger(_configuration.CreateLogger());
        }

        public ISoloLogger GetLogger(string className = null)
        {
            if (className == null)
            {
                StackFrame frame = new StackFrame(2, false);
                var declaringType = frame.GetMethod().DeclaringType;
                if (declaringType != null) className = declaringType.Name;
            }

            if (className == null)
            {
                return _serilogLogger;
            }
            return _serilogLogger.ForContext("className", className);
        }
    }
}