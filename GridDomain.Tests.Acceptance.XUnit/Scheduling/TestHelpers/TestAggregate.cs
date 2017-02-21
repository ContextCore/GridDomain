using System;
using System.Threading;
using CommonDomain.Core;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestAggregate : AggregateBase
    {
        private TestAggregate(Guid id)
        {
            Id = id;
        }

        private int timeToOkResponse;

        public void Apply(ScheduledCommandSuccessfullyProcessed @event)
        {
            timeToOkResponse = 5;
        }

        public void Apply(ScheduledCommandProcessingFailed @event)
        {
            timeToOkResponse--;
        }

        public void Apply(ScheduledCommandProcessingFailuresPlanned @event)
        {
            timeToOkResponse = @event.FailTimes;
        }

        public void Apply(TestEvent @event)
        {
            RaiseEvent(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void Success(string taskId)
        {
            ResultHolder.Add(taskId, taskId);
            RaiseEvent(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void LongTime(string taskId, TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            ResultHolder.Add(taskId, taskId);
            RaiseEvent(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void Failure(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            throw new InvalidOperationException("ohshitwaddap");
        }
    }
}