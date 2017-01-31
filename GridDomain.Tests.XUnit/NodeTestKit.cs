using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
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
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{

    public abstract class NodeTestKit : TestKit
    {
        private NodeTestFixture NodeTestFixture { get;}
        protected GridDomainNode Node => NodeTestFixture.Node;
        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture, LogEventLevel level = LogEventLevel.Information): base(fixture.GetConfig(), fixture.Name)
        {
            Serilog.Log.Logger = new XUnitAutoTestLoggerConfiguration(output,level).CreateLogger();
            NodeTestFixture = fixture;
            NodeTestFixture.System = Sys;
        }

        //do not kill Akka system on each test run
        protected override void AfterAll()
        {
        }
    }


}