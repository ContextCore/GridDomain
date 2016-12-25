using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Logging;
using GridDomain.Scheduling.Akka.Messages;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Integration
{
    public class SchedulingActor : ReceiveActor
    {
        private readonly IScheduler _scheduler;
        private readonly ISoloLogger _logger = LogManager.GetLogger();
        public SchedulingActor(IScheduler scheduler)
        {
            _logger.Debug("Scheduling actor started at path {Path}",Self.Path);
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
                var jobKey = new JobKey(msg.Key.Name, msg.Key.Group);
                _scheduler.DeleteJob(jobKey);
                _logger.Debug("Unscheduled job {Task}", msg.Key);
                Sender.Tell(new Unscheduled(msg.Key));
            }
            catch (Exception e)
            {
                _logger.Error(e,"Error while Unscheduled job {Task}", msg.Key);
                Sender.Tell(new Failure { Exception = e, Timestamp = BusinessDateTime.UtcNow });
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
            Schedule(() => QuartzJob.Create(message.Key, message.Event, message.EventMetadata), 
                           message.RunAt,
                           message.Key);
        }

        private void Schedule(Func<IJobDetail> jobFactory, DateTime runAt, ScheduleKey key)
        {
            try
            {
                _logger.Debug("Scheduling job {Task}", key);
                var job = jobFactory();
                var trigger = TriggerBuilder.Create()
                                            .WithIdentity(job.Key.Name, job.Key.Group)
                                            .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow()
                                                                      .WithRepeatCount(0))
                                            .StartAt(runAt)
                                            .Build();

                var fireTime = _scheduler.ScheduleJob(job, trigger);
                _logger.Debug("Scheduled job {Task} at {Time}", key, fireTime);
                Sender.Tell(new Scheduled(fireTime.UtcDateTime));

            }
            catch (JobPersistenceException e)
            {
                _logger.Error(e,"Error while scheduled job {Task}", key);
                if (e.InnerException?.GetType() == typeof(ObjectAlreadyExistsException))
                {
                    Sender.Tell(new AlreadyScheduled(key));
                }
            }
            catch (Exception e)
            {
                _logger.Error(e,"Error while scheduled job {Task}", key);
                Sender.Tell(new Failure { Exception = e, Timestamp = BusinessDateTime.UtcNow });
            }
        }

        protected override void PreStart()
        {
            _scheduler.Start();
            base.PreStart();
        }

        protected override void PostStop()
        {
            _scheduler.Shutdown();
            base.PostStop();
        }
    }
}