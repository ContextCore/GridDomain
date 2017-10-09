using System;
using System.Collections.Generic;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using Serilog.Events;

namespace GridDomain.Node.Configuration.Hocon
{
    public class LogConfig : IHoconConfig
    {
        private readonly Dictionary<LogEventLevel, string> _akkaLogLevels = new Dictionary<LogEventLevel, string>
                                                                       {
                                                                           {LogEventLevel.Information,"INFO"},
                                                                           {LogEventLevel.Error,"ERROR"},
                                                                           {LogEventLevel.Fatal,"ERROR"},
                                                                           {LogEventLevel.Verbose, "DEBUG"},
                                                                           {LogEventLevel.Debug,   "DEBUG"},
                                                                           {LogEventLevel.Warning,"WARNING"}
                                                                       };

        private readonly bool _includeConfig;
        private readonly Type _logActorType;

        private readonly LogEventLevel _verbosity;

        public LogConfig(LogEventLevel verbosity, Type logActorType=null, bool writeConfig = false)
        {
            _verbosity = verbosity;
            _includeConfig = writeConfig || verbosity == LogEventLevel.Verbose || verbosity == LogEventLevel.Debug;
            _logActorType = logActorType ?? typeof(SerilogLoggerActor);
        }

        public string Build()
        {
            var logLevel = _akkaLogLevels[_verbosity];
            var logConfig = @"
                #stdout-loglevel = ERROR
                loglevel=" + logLevel;
            logConfig += @"
                loggers=[""" + _logActorType.AssemblyQualifiedShortName() + @"""]

                actor.debug {" + AdditionalLogs(_verbosity) + @" 
                      unhandled = on
                }";

            if (_includeConfig)
                logConfig += @"
                log-config-on-start = on";

            return logConfig;
        }

        private object AdditionalLogs(LogEventLevel verbosity)
        {
            return verbosity == LogEventLevel.Verbose ? 
                  @"autoreceive = on
                    lifecycle = on
                    receive = on
                    router-misconfiguration = on
                    event-stream = on" : "";
        }
    }
}