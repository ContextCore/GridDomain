using System.Collections.Generic;

namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class LogConfig : IAkkaConfig
    {
        private readonly bool _includeConfig;
        private readonly string _logLevel;

        private readonly Dictionary<LogVerbosity, string> _akkaLogLevels = new Dictionary<LogVerbosity, string>
        {
            {LogVerbosity.Info, "INFO"},
            {LogVerbosity.Error, "ERROR"},
            {LogVerbosity.Trace, "DEBUG"},
            {LogVerbosity.Warning, "WARNING"}
        };

        private readonly LogVerbosity _verbosity;


        public LogConfig(LogVerbosity verbosity, bool includeConfig = true)
        {
            _verbosity = verbosity;
            _includeConfig = includeConfig;
            _logLevel = _akkaLogLevels[verbosity];
        }

        public string Build()
        {
           
            var logConfig =
                @"
                stdout-loglevel = " + _logLevel + @"
                loglevel=" + _logLevel;

            logConfig += @"
                loggers=["""+typeof(SerilogExtendedLogger).AssemblyQualifiedShortName() + @"""]

                actor.debug {"+
#if DEBUG
                    @"receive = on
                      event-stream = on "
                      +
#endif
                     @"
                     "+AdditionalLogs(_verbosity)+@" 
                      unhandled = on
                }";

            if (_includeConfig)
                    logConfig += @"
                log-config-on-start = on";

            return logConfig;
        }

        private object AdditionalLogs(LogVerbosity verbosity)
        {
            return verbosity == LogVerbosity.Trace
                ? @"autoreceive = on
                    lifecycle = on"
                : "";
        }
    }
}