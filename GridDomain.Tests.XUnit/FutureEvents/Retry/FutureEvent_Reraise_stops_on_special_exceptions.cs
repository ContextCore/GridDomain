using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents.Retry
{
    public class FutureEvent_Reraise_stops_on_special_exceptions : NodeTestKit
    {
        public FutureEvent_Reraise_stops_on_special_exceptions(ITestOutputHelper output)
    : base(output, new Reraise_fixture()) { }

        class TwoFastRetriesSettings : InMemoryRetrySettings
        {
            public TwoFastRetriesSettings() : base(2, TimeSpan.FromMilliseconds(10), new StopOnTestExceptionPolicy()) {}

            class StopOnTestExceptionPolicy : IExceptionPolicy
            {
                public bool ShouldContinue(Exception ex)
                {
                    _policyCallNumber ++;
                    _policyCallNumberChanged.SetResult(1);
                    var domainException = ex.UnwrapSingle();
                    if (domainException is TestScheduledException) return false;

                    if ((domainException as TargetInvocationException)?.InnerException is TestScheduledException)
                        return false;

                    return true;
                }
            }
        }

        private static readonly TaskCompletionSource<int> _policyCallNumberChanged = new TaskCompletionSource<int>();
        private static int _policyCallNumber;

        private class Reraise_fixture : FutureEventsFixture
        {
            public Reraise_fixture()
            {
                var cfg =  new CustomContainerConfiguration(c => c.RegisterInstance<IRetrySettings>(new TwoFastRetriesSettings()));
                Add(cfg);
            }
        }

        [Fact]
        public async Task Should_not_retry_on_exception()
        {
            //will retry 1 time
            var command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.3), Guid.NewGuid(), "test value A", 2);

            await Node.Prepare(command)
                      .Expect<JobFailed>()
                      .Execute();

            //waiting for policy call to determine should we retry failed job or not
            await _policyCallNumberChanged.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
            // job was not retried and policy was not called
            Assert.Equal(1, _policyCallNumber);
        }
    }
}