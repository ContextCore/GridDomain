using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using Quartz;
using IScheduler = Quartz.IScheduler;
using GridDomain.Transport;
using GridDomain.Transport.Extension;

namespace GridDomain.Scheduling.Akka
{
    public class SchedulingActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private readonly IScheduler _scheduler;
        private readonly IPublisher _publisher;

        public SchedulingActor()
        {
            _publisher = Context.System.GetTransport();
            _logger.Debug("Scheduling actor started at path {Path}", Self.Path);
            _scheduler = Context.System.GetExtension<SchedulingExtension>().Scheduler;
            ReceiveAsync<ScheduleCommandExecution>(Schedule);
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

        private async Task Schedule(ScheduleCommandExecution message)
        {
            ScheduleKey key = message.Key;
            try
            {
                _logger.Debug($"Scheduling job {key} for {message.Options.RunAt}");
                var job = QuartzJob.Create(message.Key, message.Command, message.CommandMetadata, message.Options);
                var trigger =
                    TriggerBuilder.Create()
                                  .WithIdentity(job.Key.Name, job.Key.Group)
                                  .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow().WithRepeatCount(0))
                                  .StartAt(message.Options.RunAt)
                                  .Build();

                var fireTime = await _scheduler.ScheduleJob(job, trigger);
                var scheduleConfirmation = new CommandExecutionScheduled(message.Command.Id, fireTime.UtcDateTime);
                Sender.Tell(scheduleConfirmation);
                _publisher.Publish(scheduleConfirmation,message.CommandMetadata);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while scheduled job {Task}", key);
                if (e is JobPersistenceException && e.InnerException?.GetType() == typeof(ObjectAlreadyExistsException))
                {
                    Sender.Tell(new AlreadyScheduled(key));
                }
                else
                    Sender.Tell(new Status.Failure(e));

                var faultId = "job_schedule_fault_" + Guid.NewGuid();
                 var fault = Fault.New(faultId,message, e, message.Command.ProcessId, typeof(SchedulingActor));
                var faultMetadata = message.CommandMetadata.CreateChild(faultId);

                _publisher.Publish(fault, faultMetadata);
            }
        }
    }
}