using System;
using Serilog;
using Serilog.Events;

namespace GridDomain.Logging
{
    public class DefaultLoggerFactory : LoggerFactory
    {
        private static readonly Lazy<SerilogLogger> LoggerFactory = new Lazy<SerilogLogger>(() => new SerilogLogger(GetConfiguration().CreateLogger()));

        private static LoggerConfiguration GetConfiguration()
        {
            var configuration = new LoggerConfiguration();
            configuration = configuration.WriteTo.Console(LogEventLevel.Information);
            return configuration;
        }

        public override ISoloLogger GetLogger(string className = null)
        {
            className = className ?? GetClassName();
            return LoggerFactory.Value.ForContext("className", className);

        }
    }
}