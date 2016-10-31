using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Snapshots
{
    [TestFixture]
    class Given_snapshot_aggregate_Should_recover : SampleDomainCommandExecutionTests
    {
        public Given_snapshot_aggregate_Should_recover(): base(true) {}

        [Test]
        public void Test()
        {
            var aggr = 
        }
    }

    [TestFixture]
    class Given_snapshot_saga_Should_recover
    {
    }

    [TestFixture]
    class Aggregate_Should_save_snapshots_each_10_messages
    {
    }


    [TestFixture]
    class Saga_Should_save_snapshots_each_10_messages
    {
    }

    [TestFixture]
    class Saga_Should_save_snapshots_on_message_process_if_activity_is_low
    {
    }


    [TestFixture]
    class Aggregate_Should_save_snapshots_on_message_process_if_activity_is_low
    {
    }
}
