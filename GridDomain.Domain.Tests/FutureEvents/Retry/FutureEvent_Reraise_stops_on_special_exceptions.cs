using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents.Retry
{
    [TestFixture]
    public class FutureEvent_Reraise_stops_on_special_exceptions : FutureEventsTest_InMemory
    {

        class TwoFastRetriesSettings : InMemoryRetrySettings
        {
            public TwoFastRetriesSettings():base(2,TimeSpan.FromMilliseconds(10),new StopOnTestExceptionPolicy())
            {
                
            }


            class StopOnTestExceptionPolicy : IExceptionPolicy
            {
                public bool ShouldContinue(Exception ex)
                {
                    _policyCallNumber ++;
                    var domainException = ex.UnwrapSingle();
                    if (domainException is TestScheduledException)
                        return false;
                    if ((domainException as TargetInvocationException)?.InnerException is TestScheduledException)
                        return false;

                    return true;
                }
            }
        }

        private static int _policyCallNumber;

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => c.Register(base.CreateConfiguration()),
                                                    c => c.RegisterInstance<IRetrySettings>(new TwoFastRetriesSettings()));
      
        }

        [Test]
        public async Task Should_not_retry_on_exception()
        {
            //will retry 1 time
            var _command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.1), Guid.NewGuid(), "test value A",2);

            var waiter =  GridNode.NewWaiter()
                                  .Expect<JobFailed>()
                                  .Create();

            GridNode.Execute(_command);

            var res = await waiter;

            Thread.Sleep(5000);

            // job was not retried and policy was not called
            Assert.AreEqual(1, _policyCallNumber);
        }
    }
}