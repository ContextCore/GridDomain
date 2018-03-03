using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using Serilog;

namespace GridDomain.Scheduling.Akka
{
    /// <summary>
    ///     Listening to scheduling events from aggregates and modify quartz jobs accordinally
    /// </summary>
    internal class FutureEventsSchedulingMessageHandler : IHandler<FutureEventScheduledEvent>,
                                                          IHandler<FutureEventCanceledEvent>
    {
        private readonly IActorRef _schedulerActorRef;
        private readonly ProcessEntry _schedulingFutureEventProcessEntry;
        private readonly ILogger _logger;

        public FutureEventsSchedulingMessageHandler(IActorRef schedulingActor, ILogger log)
        {
            _logger = log;
            _schedulerActorRef = schedulingActor;

            _schedulingFutureEventProcessEntry = new ProcessEntry(GetType()
                                                                      .Name,
                                                                  "Scheduling raise future event command",
                                                                  "FutureEventScheduled event occured");
        }

        public Task Handle(FutureEventCanceledEvent evt, IMessageMetadata metadata)
        {
            var key = CreateScheduleKey(evt.FutureEventId, evt.SourceId, evt.SourceName);
            return _schedulerActorRef.Ask<object>(new Unschedule(key))
                .ContinueWith(t =>
                              {
                                  switch (t.Result)
                                  {
                                      case Unscheduled unsched: break;
                                      case Failure f:
                                      {
                                          _logger.Error(f.Exception, "Error occured during unscheduling event");
                                          break;
                                      }
                                      default:
                                      {
                                          _logger.Error("Unexpected message received during unscheduling event confirmation wait: {msg}",t.Result);
                                          break;
                                      }
                                  }
                              });
        }

        public Task Handle(FutureEventScheduledEvent message, IMessageMetadata messageMetadata)
        {
            var scheduleId = message.Id;
            var succesEventType = message.Event.GetType();

            var description = $"Source {message.SourceName} id={message.SourceId} scheduled future event {message.Event.GetType().Name} id={scheduleId} o n{message.RaiseTime}\r\n";

            var scheduleKey = CreateScheduleKey(scheduleId, message.SourceId, message.SourceName, description);

            var command = new RaiseScheduledDomainEventCommand(message.Id, message.SourceId, Guid.NewGuid().ToString());
            var metadata = messageMetadata.CreateChild(command.Id, _schedulingFutureEventProcessEntry);

            var scheduleEvent = new ScheduleCommandExecution(command,
                                                             scheduleKey,
                                                             ExecutionOptions.ForCommand(message.RaiseTime, succesEventType),
                                                             metadata);

            return _schedulerActorRef.Ask<object>(scheduleEvent).ContinueWith(t =>
                                                                              {
                                                                                  switch (t.Result)
                                                                                  {
                                                                                      case CommandExecutionScheduled sched: break;
                                                                                      case Failure f:
                                                                                      {
                                                                                          _logger.Error(f.Exception, "Error occured during scheduling event");
                                                                                          break;
                                                                                      }
                                                                                      default:
                                                                                      {
                                                                                          _logger.Error("Unexpected message received during scheduling event confirmation wait: {msg}",t.Result);
                                                                                          break;
                                                                                      }
                                                                                  }
                                                                              });
        }

        internal static ScheduleKey CreateScheduleKey(string scheduleId, string sourceId, string sourceName, string description = null)
        {
            return new ScheduleKey($"{scheduleId:N}",
                                   $"{sourceName}_{sourceId:N}_future_events",
                                   description);
        }
    }
}