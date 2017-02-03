using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.XUnit.FutureEvents.Retry
{
    
    public class FutureEvent_regular_Reraise : FutureEventsTest_InMemory
    {

        class TwoFastRetriesSettings : InMemoryRetrySettings
        {
            public TwoFastRetriesSettings():base(2,TimeSpan.FromMilliseconds(10))
            {
                
            }
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => c.Register(base.CreateConfiguration()),
                c => c.RegisterInstance<IRetrySettings>(new TwoFastRetriesSettings()));
      
        }

        [Fact]
        public async Task Should_retry_on_exception()
        {
            //will retry 1 time
            var command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.1), Guid.NewGuid(), "test value A",1);

            var waiter = GridNode.Prepare(command)
                                 .Expect<JobFailed>()
                                 .And<JobSucceeded>()
                                 .And<TestErrorDomainEvent>()
                                 .Execute();

            var res = await waiter;

           Assert.Equal(command.Value, res.Message<TestErrorDomainEvent>().Value);
        }
    }
}