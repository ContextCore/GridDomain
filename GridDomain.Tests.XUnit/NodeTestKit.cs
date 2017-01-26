using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public abstract class NodeTestKit : TestKit
    {
        private NodeTestFixture NodeTestFixture { get; }
        protected GridDomainNode Node => NodeTestFixture.GridNode;
        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture): base(fixture.GetConfig(), fixture.Name)
        {
            Serilog.Log.Logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
            NodeTestFixture = fixture;
            NodeTestFixture.ExternalSystem = Sys;
        }

        //do not kill Akka system on each test run
        protected override void AfterAll()
        {
        }
    }
}