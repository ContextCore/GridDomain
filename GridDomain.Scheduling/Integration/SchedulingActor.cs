using System;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Integration
{
    //TODO::VZ:: receive actor
    public class SchedulingActor : ReceiveActor
    {
        private readonly IScheduler _scheduler;
        public SchedulingActor(IScheduler scheduler)
        {
            _scheduler = scheduler;
            Receive<ScheduleCommand>(message => Schedule(message));
            Receive<Unschedule>(message => Unschedule(message));
        }

        private void Unschedule(Unschedule msg)
        {
            try
            {
                var jobKey = new JobKey(msg.Key.Name, msg.Key.Group);
                _scheduler.DeleteJob(jobKey);
                Sender.Tell(new Unscheduled(msg.Key));
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTime.UtcNow });
            }
        }

        private void Schedule(ScheduleCommand scheduleCommand)
        {
            try
            {
                var job = QuartzJob.Create(scheduleCommand.Key, scheduleCommand.Command, scheduleCommand.Options).Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(job.Key.Name, job.Key.Group)
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow().WithRepeatCount(0))
                    .StartAt(scheduleCommand.Options.RunAt)
                    .Build();
                var fireTime = _scheduler.ScheduleJob(job, trigger);
                Sender.Tell(new Scheduled(fireTime.UtcDateTime));
            }
            catch (JobPersistenceException e)
            {
                if (e.InnerException.GetType() == typeof(ObjectAlreadyExistsException))
                {
                    Sender.Tell(new AlreadyScheduled(scheduleCommand.Key));
                }
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTime.UtcNow });
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