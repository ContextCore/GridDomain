using System;
using Akka;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Integration
{
    public class SchedulingActor : ActorBase
    {
        private readonly IScheduler _scheduler;
        public SchedulingActor(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        protected override bool Receive(object message)
        {
            return message.Match()
                .With<Schedule>(Schedule)
                .With<Unschedule>(Unschedule)
                .WasHandled;
        }

        private void Unschedule(Unschedule msg)
        {
            try
            {
                var jobKey = new JobKey(msg.TaskId, msg.Group);
                _scheduler.DeleteJob(jobKey);
                Sender.Tell(new Unscheduled(msg.TaskId));
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTime.UtcNow });
            }
        }

        private void Schedule(Schedule scheduleRequest)
        {
            try
            {
                var jobKey = new JobKey(scheduleRequest.Command.TaskId, scheduleRequest.Command.Group);
                var job = QuartzJob.Create(jobKey, scheduleRequest.Command, scheduleRequest.ExecutionTimeout).Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(job.Key.Name, job.Key.Group)
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow().WithRepeatCount(0))
                    .StartAt(scheduleRequest.RunAt)
                    .Build();
                var fireTime = _scheduler.ScheduleJob(job, trigger);
                Sender.Tell(new Scheduled(fireTime.UtcDateTime));
            }
            catch (JobPersistenceException e)
            {
                if (e.InnerException.GetType() == typeof(ObjectAlreadyExistsException))
                {
                    Sender.Tell(new AlreadyScheduled(scheduleRequest.Command.TaskId, scheduleRequest.Command.Group));
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