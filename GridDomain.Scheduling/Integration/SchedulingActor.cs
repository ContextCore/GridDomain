using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Scheduling.Akka.Messages;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Integration
{
    public class SchedulingActor : ReceiveActor
    {
        private readonly IScheduler _scheduler;
        public SchedulingActor(IScheduler scheduler)
        {
            _scheduler = scheduler;
            Receive<ScheduleCommand>(message => Schedule(message));
            Receive<ScheduleMessage>(message => Schedule(message));
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
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTimeFacade.UtcNow });
            }
        }

        private void Schedule(ScheduleCommand message)
        {
            Schedule(() => QuartzJob.Create(message.Key, message.Command, message.Options), message.Options.RunAt, message.Key);
        }

        private void Schedule(ScheduleMessage message)
        {
            Schedule(() => QuartzJob.Create(message.Key, message.Event), message.RunAt, message.Key);
        }

        private void Schedule(Func<IJobDetail> jobFactory, DateTime runAt, ScheduleKey key)
        {
            try
            {
                var job = jobFactory();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(job.Key.Name, job.Key.Group)
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow().WithRepeatCount(0))
                    .StartAt(runAt)
                    .Build();
                var fireTime = _scheduler.ScheduleJob(job, trigger);
                Sender.Tell(new Scheduled(fireTime.UtcDateTime));

            }
            catch (JobPersistenceException e)
            {
                if (e.InnerException.GetType() == typeof(ObjectAlreadyExistsException))
                {
                    Sender.Tell(new AlreadyScheduled(key));
                }
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTimeFacade.UtcNow });
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