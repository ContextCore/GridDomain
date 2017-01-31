using System.Collections.Generic;
using Akka.Event;
using Akka.Logger.Serilog;

namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class LogConfig : IAkkaConfig
    {
        private readonly bool _includeConfig;

        private readonly Dictionary<LogLevel, string> _akkaLogLevels = new Dictionary<LogLevel, string>
        {
            {LogLevel.InfoLevel,    "INFO"},
            {LogLevel.ErrorLevel,   "ERROR"},
            {LogLevel.DebugLevel,   "DEBUG"},
            {LogLevel.WarningLevel, "WARNING"}
        };

        private readonly LogLevel _verbosity;

        public LogConfig(LogLevel verbosity, bool includeConfig = true)
        {
            _verbosity = verbosity;
            _includeConfig = includeConfig;
        }

        public string Build()
        {
            var logLevel = _akkaLogLevels[_verbosity];
            var logConfig =
                @"
                stdout-loglevel = " + logLevel + @"
                loglevel=" + logLevel;
            logConfig += @"
                loggers=["""+typeof(SerilogLogger).AssemblyQualifiedShortName() + @"""]

                actor.debug {"+AdditionalLogs(_verbosity)+ @" 
                      unhandled = on
                }";
            
            if (_includeConfig)
                    logConfig += @"
                log-config-on-start = on";

            return logConfig;
        }

        private object AdditionalLogs(LogLevel verbosity)
        {
            return verbosity == LogLevel.DebugLevel
                ? @"#autoreceive = on
                    #lifecycle = on
                    #receive = on
                    #router-misconfiguration = on
                    #event-stream = on"
                : "";
        }
    }
}