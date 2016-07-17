using Serilog;

namespace GridDomain.Logging
{
    public class DefaultLoggerFactory : ILoggerFactory
    {
        private readonly LoggerConfiguration _configuration;

        public DefaultLoggerFactory(LoggerConfiguration configuration= null)
        {
            _configuration = configuration ?? new DefaultLoggerConfiguration();
            Serilog.Log.Logger = _configuration.CreateLogger();
        }

        public ISoloLogger GetLogger()
        {
            return new SerilogLogger(_configuration.CreateLogger());
        }
    }

    public class DefaultLoggerConfiguration : LoggerConfiguration
    {
        public DefaultLoggerConfiguration()
        {
            this.WriteTo.Console();
        }
    }
}