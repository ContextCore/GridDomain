using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.SampleDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class SampleDomainCommandExecutionTests : ExtendedNodeCommandTest
    {
      

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);
        protected override IContainerConfiguration CreateConfiguration()
        {
            return new SampleDomainContainerConfiguration();
        }

        protected override IMessageRouteMap CreateMap()
        {
            var container = new UnityContainer();
            container.Register(CreateConfiguration());
            return new SampleRouteMap();
        }

        public SampleDomainCommandExecutionTests(bool inMemory, AkkaConfiguration config = null) : base(inMemory, config)
        {
        }

        public SampleDomainCommandExecutionTests()
        {
        }
    }
}