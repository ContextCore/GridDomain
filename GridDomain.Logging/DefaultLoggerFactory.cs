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

        public ISoloLogger GetLogger()
        {
            return _serilogLogger;
        }
    }
}