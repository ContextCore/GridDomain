using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    public abstract class ExtendedNodeCommandTest : NodeCommandsTest
    {
        private readonly bool _inMemory;
        protected abstract IContainerConfiguration CreateConfiguration();
        protected abstract IMessageRouteMap CreateMap();
        protected ExtendedNodeCommandTest(bool inMemory) : 
            base( inMemory ? new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig():
                new AutoTestAkkaConfiguration().ToStandAloneSystemConfig()
                , "TestSystem", !inMemory)
        {
            _inMemory = inMemory;
        }

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var actorSystem = _inMemory ? Sys : ActorSystemFactory.CreateActorSystem(akkaConf);
            return new GridDomainNode(CreateConfiguration(),CreateMap(),TransportMode.Standalone, actorSystem);
        }
    }
}