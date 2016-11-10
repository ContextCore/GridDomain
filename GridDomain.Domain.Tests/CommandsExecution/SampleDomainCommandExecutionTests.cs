using System;
using System.Diagnostics;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.SampleDomain;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution
{
    public class SampleDomainCommandExecutionTests : ExtendedNodeCommandTest
    {
        protected IPublisher Publisher => GridNode.Transport;

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);
        protected override IContainerConfiguration CreateConfiguration()
        {
            return new SampleDomainContainerConfiguration();
        }

        protected override IMessageRouteMap CreateMap()
        {
            var container = new UnityContainer();
            container.Register(CreateConfiguration());
            return new SampleRouteMap(container);
        }

        public SampleDomainCommandExecutionTests(bool inMemory) : base(inMemory)
        {
        }

        public SampleDomainCommandExecutionTests()
        {
        }
    }
}