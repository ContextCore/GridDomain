using Serilog;

namespace GridDomain.Logging
{
    public class DefaultLoggerConfiguration : LoggerConfiguration
    {
        public DefaultLoggerConfiguration()
        {
            this.WriteTo.Console();
        }
    }
}