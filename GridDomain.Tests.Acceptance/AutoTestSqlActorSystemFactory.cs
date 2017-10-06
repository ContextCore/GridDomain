using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Acceptance {
    public class AutoTestSqlActorSystemFactory : IActorSystemFactory
    {
        public ActorSystem Create()
        {
            return AutoTestAkkaConfiguration.Default.CreateSystem(AutoTestNodeDbConfiguration.Default);
        }
    }
}