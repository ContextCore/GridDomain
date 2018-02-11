using System;
using System.Threading;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestAggregate : ConventionAggregate
    {

        private TestAggregate(string id) : base(id) {}

        public void Apply(ScheduledCommandSuccessfullyProcessed @event)
        {
        }

        public void Apply(TestEvent @event)
        {
            Produce(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void Success(string taskId)
        {
            ResultHolder.Add(taskId, taskId);
            Produce(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void LongTime(string taskId, TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            ResultHolder.Add(taskId, taskId);
            Produce(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void Failure(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            throw new InvalidOperationException("ohshitwaddap");
        }
    }
}