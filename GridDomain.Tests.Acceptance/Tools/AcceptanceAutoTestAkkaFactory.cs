using Akka.Actor;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Persistence.Sql;
using Serilog.Events;

namespace GridDomain.Tests.Acceptance.Tools {
    public class AcceptanceAutoTestAkkaFactory : IActorSystemFactory
    {
        private readonly LogEventLevel _verbosity;

        public AcceptanceAutoTestAkkaFactory(LogEventLevel verbosity = LogEventLevel.Debug)
        {
            _verbosity = verbosity;
        }
        public ActorSystem CreateSystem()
        {
            return new AcceptanceAutoTestNodeConfiguration(_verbosity).CreateSystem(AutoTestNodeDbConfiguration.Default);
        }
    }
}