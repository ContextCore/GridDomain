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
        protected abstract IContainerConfiguration CreateConfiguration();
        protected abstract IMessageRouteMap CreateMap();
        protected ExtendedNodeCommandTest(bool inMemory) : 
            base( inMemory ? new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig():
                new AutoTestAkkaConfiguration().ToStandAloneSystemConfig()
                , "TestInMemorySystem", !inMemory)
        {
        }

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            return new GridDomainNode(CreateConfiguration(),CreateMap(),TransportMode.Standalone,Sys);
        }
    }
}