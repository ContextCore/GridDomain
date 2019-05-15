using System;
using System.Text;
using Akka.Event;

namespace GridDomain.Node.Akka.Configuration.Hocon
{
    public class LogConfig : IHoconConfig
    {
        private static string GetVerbosity(LogLevel lvl)
        {
            switch (lvl)
            {
                case LogLevel.ErrorLevel:return "ERROR";
                case LogLevel.WarningLevel:return "WARNING";
                case LogLevel.InfoLevel:return "INFO";
                case LogLevel.DebugLevel:return "DEBUG";
                default:return "DEBUG";
            }
        }
        private static string GetFlag(bool value)
        {
            return value ? "on" : "off";
        }

        private readonly string _config;
        public readonly LogLevel LogEventLevel;

        public static LogConfig All(Type logger = null)
        {
            return new LogConfig(LogLevel.DebugLevel, true, true, true, true, true, true, true, logger);
        }

        public LogConfig(LogLevel verbosity = LogLevel.DebugLevel,
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
            if(logActorType != null)
             builder.AppendLine($"akka.loggers  = [\"{logActorType.AssemblyQualifiedShortName()}\"]");

            _config = builder.ToString();
        }

        public string Build()
        {
            return _config;
        }
    }
}