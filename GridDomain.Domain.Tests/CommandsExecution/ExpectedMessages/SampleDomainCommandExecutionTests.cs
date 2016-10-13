using System;
using System.Diagnostics;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.SampleDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.CommandsExecution
{
    public class SampleDomainCommandExecutionTests : ExtendedNodeCommandTest
    {
        protected override TimeSpan Timeout => Debugger.IsAttached
            ? TimeSpan.FromMinutes(10)
            : TimeSpan.FromSeconds(5);

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

        public SampleDomainCommandExecutionTests() : base(true)
        {
        }

        public SampleDomainCommandExecutionTests(bool inMemory) : base(inMemory)
        {
        }
    }
}