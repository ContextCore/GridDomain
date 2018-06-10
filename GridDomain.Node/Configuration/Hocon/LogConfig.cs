using System;
using System.Collections.Generic;
using Akka.Configuration;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Logging;
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
            _includeConfig = writeConfig || verbosity == LogEventLevel.Verbose;
            _logActorType = logActorType ?? typeof(SerilogLoggerActor);
        }

        public Config Build()
        {
            var logLevel = _akkaLogLevels[_verbosity]; 
            var logConfig = @"
                akka.stdout-loglevel = " + logLevel +@"
                akka.loglevel=" + logLevel + @"
                akka.loggers=[""" + _logActorType.AssemblyQualifiedShortName() + @"""]

                akka.actor.debug {" + AdditionalLogs(_verbosity) + @" 
                      unhandled = on
                }";

            if (_includeConfig)
                logConfig += @"
                akka.log-config-on-start = on";

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