using Akka.Actor;
using Serilog;

namespace GridDomain.Node.Logging {
    public static class LoggingExtensions
    {
        public static LoggingExtension InitSerilogExtension(this ActorSystem system, ILogger log, string name=null)
        {
            return (LoggingExtension) system.RegisterExtension(new LoggingExtensionProvider(log,name));
        }

      
    }
}