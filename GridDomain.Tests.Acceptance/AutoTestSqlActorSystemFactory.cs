using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Acceptance {
    public class AutoTestSqlActorSystemFactory : IActorSystemFactory
    {
        public ActorSystem CreateSystem()
        {
            return AutoTestNodeConfiguration.Default.CreateSystem(AutoTestNodeDbConfiguration.Default);
        }
    }
}