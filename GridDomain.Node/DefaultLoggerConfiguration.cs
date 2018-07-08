using System.IO;
using System.Linq;
using Akka.Event;
using Serilog;
using Serilog.Events;

namespace GridDomain.Node
{
    public class DefaultLoggerConfiguration : LoggerConfiguration
    {
        public DefaultLoggerConfiguration(LogEventLevel level = LogEventLevel.Verbose, string fileName = null)
        {
            this.Default(level);
            this.WriteToFile(level, fileName);
        }
    }
}