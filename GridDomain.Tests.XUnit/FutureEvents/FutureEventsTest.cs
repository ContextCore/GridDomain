using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    public class FutureEventsTest : NodeTestKit
    {
        public FutureEventsTest(ITestOutputHelper output) : base(output, new FutureEventsFixture()) {}
    }
}