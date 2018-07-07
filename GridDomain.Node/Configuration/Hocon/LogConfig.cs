using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Akka.Configuration;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Logging;
using Serilog.Events;

namespace GridDomain.Node.Configuration.Hocon
{
    public class LogConfig : IHoconConfig
    {
        private static string GetVerbosity(LogEventLevel lvl)
        {
            switch (lvl)
            {
                case LogEventLevel.Fatal:return "ERROR";
                case LogEventLevel.Error:return "WARNING";
                case LogEventLevel.Warning:return "ERROR";
                case LogEventLevel.Information:return "INFO";
                case LogEventLevel.Debug:return "DEBUG";
                case LogEventLevel.Verbose:return "DEBUG";
                default:return "DEBUG";
            }
        }
        private static string GetFlag(bool value)
        {
            return value ? "on" : "off";
        }

        private readonly string _config;
        public readonly LogEventLevel LogEventLevel;

        public static LogConfig All()
        {
            return new LogConfig(LogEventLevel.Verbose,true,true,true,true,true,true,true);
        }

        public LogConfig(LogEventLevel verbosity = LogEventLevel.Verbose,
                         bool autoreceive = false,
                         bool lifecycle = false,
                         bool receive = false,
                         bool routerConfig = false,
                         bool eventStream = false,
                         bool unhandled = false,
                         bool configOnStart = false,
                         Type logActorType = null)
        {
            LogEventLevel = verbosity;
            var builder = new StringBuilder();
            builder.AppendLine($"akka.actor.debug.autoreceive = {GetFlag(autoreceive)}");
            builder.AppendLine($"akka.actor.debug.lifecycle = {GetFlag(lifecycle)}");
            builder.AppendLine($"akka.actor.debug.receive = {GetFlag(receive)}");
            builder.AppendLine($"akka.actor.debug.router-misconfiguration = {GetFlag(routerConfig)}");
            builder.AppendLine($"akka.actor.debug.event-stream = {GetFlag(eventStream)}");
            builder.AppendLine($"akka.actor.debug.unhandled = {GetFlag(unhandled)}");
            builder.AppendLine($"akka.log-config-on-start  = {GetFlag(configOnStart)}");
            builder.AppendLine($"akka.loglevel  = {GetVerbosity(verbosity)}");
            builder.AppendLine($"akka.loggers  = [\"{(logActorType ?? typeof(SerilogLoggerActor)).AssemblyQualifiedShortName()}\"]");

            _config = builder.ToString();
        }

        public string Build()
        {
            return _config;
        }
    }
}