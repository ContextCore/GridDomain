using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestAggregate : Aggregate
    {
        private int _timeToOkResponse;

        private TestAggregate(Guid id) : base(id) {}

        public void Apply(ScheduledCommandSuccessfullyProcessed @event)
        {
            _timeToOkResponse = 5;
        }

        public void Apply(ScheduledCommandProcessingFailed @event)
        {
            _timeToOkResponse--;
        }

        public void Apply(TestEvent @event)
        {
            RaiseEvent(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void Success(string taskId)
        {
            ResultHolder.Add(taskId, taskId);
            Emit(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void LongTime(string taskId, TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            ResultHolder.Add(taskId, taskId);
            Emit(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void Failure(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            throw new InvalidOperationException("ohshitwaddap");
        }
    }
}