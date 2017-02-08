using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.TestKit.Xunit2;
using Akka.TestKit.Xunit2.Internals;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.EventsUpgrade;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;
using Microsoft.Practices.Unity;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public abstract class NodeTestKit : TestKit
    {
        private readonly Lazy<Task<GridDomainNode>> _lazyNode;
        protected GridDomainNode Node => _lazyNode.Value.Result;

        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture)
                            : base(fixture.GetConfig(), fixture.Name)
        {
            fixture.System = Sys;
            fixture.Output = output;
            _lazyNode = new Lazy<Task<GridDomainNode>>(async () => await fixture.CreateNode().ConfigureAwait(false));
        }

        //do not kill Akka system on each test run
       // protected override void AfterAll() {}
    }
}