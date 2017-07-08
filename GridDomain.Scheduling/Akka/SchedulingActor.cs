using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Akka
{
    public class SchedulingActor : ReceiveActor
    {
        public const string RegistrationName = nameof(SchedulingActor);
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private readonly IScheduler _scheduler;

        public SchedulingActor(IScheduler scheduler)
        {
            _logger.Debug("Scheduling actor started at path {Path}", Self.Path);
            _scheduler = scheduler;
            Receive<ScheduleCommand>(message => Schedule(message));
            Receive<ScheduleMessage>(message => Schedule(message));
            Receive<Unschedule>(message => Unschedule(message));
        }

        private void Unschedule(Unschedule msg)
        {
            try
            {
                _logger.Debug("Unscheduling job {Task}", msg.Key);
                _scheduler.DeleteJob(msg.Key.ToJobKey());
                _logger.Debug("Unscheduled job {Task}", msg.Key);
                Sender.Tell(new Unscheduled(msg.Key));
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while Unscheduled job {Task}", msg.Key);
                Sender.Tell(new Failure {Exception = e, Timestamp = BusinessDateTime.UtcNow});
            }
        }

        private void Schedule(ScheduleCommand message)
        {
            Schedule(() => QuartzJob.Create(message.Key, message.Command, message.CommandMetadata, message.Options),
                     message.Options.RunAt,
                     message.Key);
        }

        private void Schedule(ScheduleMessage message)
        {
            Schedule(() => QuartzJob.Create(message.Key, message.Event, message.EventMetadata), message.RunAt, message.Key);
        }

        private void Schedule(Func<IJobDetail> jobFactory, DateTime runAt, ScheduleKey key)
        {
            try
            {
                var job = jobFactory();
                var trigger =
                    TriggerBuilder.Create()
                                  .WithIdentity(job.Key.Name, job.Key.Group)
                                  .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow().WithRepeatCount(0))
                                  .StartAt(runAt)
                                  .Build();

                var fireTime = _scheduler.ScheduleJob(job, trigger);
                Sender.Tell(new Scheduled(fireTime.UtcDateTime));
            }
            catch (JobPersistenceException e)
            {
                _logger.Error(e, "Error while scheduled job {Task}", key);
                if (e.InnerException?.GetType() == typeof(ObjectAlreadyExistsException))
                    Sender.Tell(new AlreadyScheduled(key));
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while scheduled job {Task}", key);
                Sender.Tell(new Failure {Exception = e, Timestamp = BusinessDateTime.UtcNow});
            }
        }
    }
}