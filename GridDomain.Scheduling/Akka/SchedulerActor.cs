using System;
using Akka;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Akka
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
                Sender.Tell(new TaskRemoved());
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
                var job = QuartzJob.Create(msg.Task).Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(msg.Task.Request.TaskId)
                    .ForJob(job)
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow())
                    .StartAt(msg.Task.RunAt)
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