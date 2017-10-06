using Akka.Actor;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Tests.Acceptance.Tools {
    public class AcceptanceAutoTestAkkaFactory : IActorSystemFactory
    {
        private readonly LogLevel _verbosity;

        public AcceptanceAutoTestAkkaFactory(LogLevel verbosity = LogLevel.DebugLevel)
        {
            _verbosity = verbosity;
        }
        public ActorSystem Create()
        {
            return new AcceptanceAutoTestAkkaConfiguration(_verbosity).CreateSystem(AutoTestNodeDbConfiguration.Default);
        }


    }
}