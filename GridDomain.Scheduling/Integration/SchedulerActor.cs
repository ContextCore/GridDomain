using System;
using Akka;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Integration
{
    public class SchedulerActor : ActorBase
    {
        private readonly IScheduler _scheduler;

        public SchedulerActor(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        protected override bool Receive(object message)
        {
            return message.Match()
                .With<AddTask>(AddTask)
                .With<RemoveTask>(RemoveTask)
                .WasHandled;
        }

        private void RemoveTask(RemoveTask msg)
        {
            try
            {
                var jobKey = new JobKey(msg.TaskId);
                _scheduler.DeleteJob(jobKey);
                Sender.Tell(new TaskRemoved(msg.TaskId));
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTime.UtcNow });
            }
        }

        private void AddTask(AddTask msg)
        {
            try
            {
                var job = QuartzJob.Create(msg.Request, msg.ExecutionTimeout).Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(msg.Request.TaskId)
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow())
                    .StartAt(msg.RunAt)
                    .Build();
                var fireTime = _scheduler.ScheduleJob(job, trigger);
                Sender.Tell(new TaskAdded(fireTime.UtcDateTime));
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