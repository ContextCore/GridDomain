using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Actors.Aggregates.Exceptions;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents.Retry
{
    public class FutureEvent_Reraise_stops_on_special_exceptions : NodeTestKit
    {
        public FutureEvent_Reraise_stops_on_special_exceptions(ITestOutputHelper output)
            : base(output, new FutureEventsFixture(output,new InMemoryRetrySettings(2,
                TimeSpan.FromMilliseconds(10),
                new StopOnTestExceptionPolicy(
                    new XUnitAutoTestLoggerConfiguration(output, LogEventLevel.Information)
                        .CreateLogger())))) {}

        private static readonly TaskCompletionSource<int> _policyCallNumberChanged = new TaskCompletionSource<int>();
        private static int _policyCallNumber;


            private class StopOnTestExceptionPolicy : IExceptionPolicy
            {
                private readonly ILogger _log;

                public StopOnTestExceptionPolicy(ILogger log)
                {
                    _log = log;
                }

                public bool ShouldContinue(Exception ex)
                {
                    _log.Information("Should continue {code} called from Thread {thread} with stack trace {trace}",
                                     GetHashCode(),
                                     Thread.CurrentThread.ManagedThreadId,
                                     Environment.CurrentManagedThreadId);

                    _policyCallNumber++;
                    _policyCallNumberChanged.SetResult(1);

                    var businessException = ex.UnwrapSingle();
                    if(businessException is CommandExecutionFailedException && businessException.InnerException is ScheduledEventNotFoundException)
                        return false;

                    return true;
                }
            }

        [Fact]
        public async Task Should_not_retry_on_exception()
        {
            //will retry 1 time
            var command = new PlanBoomCommand(Guid.NewGuid(),DateTime.Now.AddSeconds(0.2));

            await Node.Prepare(command)
                      .Expect<JobFailed>()
                      .Execute();

            //give some time to scheduler listeners to proceed
            await Task.Delay(1000);

            //waiting for policy call to determine should we retry failed job or not
            await _policyCallNumberChanged.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
            // job was not retried and policy was not called
            Assert.Equal(1, _policyCallNumber);
        }
    }
}