using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.FutureEvents
{
    public class FutureEventsTest_InMemory : FutureEventsTest
    {
        public FutureEventsTest_InMemory(): base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "testSystem", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        protected override IQuartzConfig CreateQuartzConfig()
        {
            return new InMemoryQuartzConfig();
        }
    }
}
