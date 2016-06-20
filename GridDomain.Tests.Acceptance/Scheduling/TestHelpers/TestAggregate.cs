using System;
using System.Threading;
using CommonDomain.Core;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestAggregate : AggregateBase
    {
        private TestAggregate(Guid id)
        {
            Id = id;
        }

        public void Apply(ScheduledCommandSuccessfullyProcessed @event)
        {

        }

        public void Apply(ScheduledCommandProcessingFailed @event)
        {

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