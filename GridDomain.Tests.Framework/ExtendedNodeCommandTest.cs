using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.Framework
{
    public abstract class ExtendedNodeCommandTest : NodeCommandsTest
    {
        protected readonly bool InMemory;
        protected abstract IContainerConfiguration CreateConfiguration();
        protected abstract IMessageRouteMap CreateMap();

        protected ExtendedNodeCommandTest(bool inMemory) : 
            base( inMemory ? new AutoTestAkkaConfiguration(AkkaConfiguration.LogVerbosity.Trace).ToStandAloneInMemorySystemConfig():
                new AutoTestAkkaConfiguration().ToStandAloneSystemConfig()
                , "TestSystem", !inMemory)
        {
            InMemory = inMemory;
        }

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var actorSystem = InMemory ? Sys : ActorSystemFactory.CreateActorSystem(akkaConf);
            return new GridDomainNode(CreateConfiguration(),CreateMap(),TransportMode.Standalone, actorSystem);
        }
    }
}