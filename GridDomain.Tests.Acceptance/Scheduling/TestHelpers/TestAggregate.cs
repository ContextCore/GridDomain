using System;
using System.Threading;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{

    public class FailIfPlannedCommand : Command
    {
        public Guid AggregateId { get;}
        public TimeSpan Timeout { get; }

        public FailIfPlannedCommand(Guid aggregateId,TimeSpan timeout = default(TimeSpan))
        {
            AggregateId = aggregateId;
            Timeout = timeout;
        }
    }

    public class TestAggregate : AggregateBase
    {
        private TestAggregate(Guid id)
        {
            Id = id;
        }

        public int timeToOkResponse;

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

        public void PlanFailures(int times)
        {
            RaiseEvent(new ScheduledCommandProcessingFailuresPlanned(Id,times));
        }

        public void FailIfPlanned(TimeSpan timeout)
        {
            Thread.Sleep(timeout);

            if(timeToOkResponse == 0)
                RaiseEvent(new ScheduledCommandSuccessfullyProcessed(Id));
            var ex = new PlannedFailureException(timeToOkResponse);

            RaiseEvent(new ScheduledCommandProcessingFailed(Id,ex));

            throw ex;
        }

        public void Failure(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            throw new InvalidOperationException("ohshitwaddap");
        }
    }

    public class PlannedFailureException : Exception
    {
        public int TimeToOkResponse { get; }

        public PlannedFailureException(int timeToOkResponse)
        {
            TimeToOkResponse = timeToOkResponse;
        }
    }
}