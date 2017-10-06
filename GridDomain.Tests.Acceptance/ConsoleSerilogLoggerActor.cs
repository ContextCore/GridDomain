using GridDomain.Node;
using GridDomain.Node.Actors.Serilog;

namespace GridDomain.Tests.Acceptance {
    public class ConsoleSerilogLoggerActor : SerilogLoggerActor
    {
        public ConsoleSerilogLoggerActor():base(new DefaultLoggerConfiguration().CreateLogger())
        {
            
        }
    }
}