using System;
using System.Collections.Specialized;
using Akka;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using Quartz;
using Quartz.Impl;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Akka
{
    public class SchedulerActor : ActorBase
    {
        private readonly IScheduler _scheduler;

        public SchedulerActor()
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = "TestScheduler",
                ["quartz.scheduler.instanceId"] = "instance_one",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = "true",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                ["quartz.dataSource.default.connectionString"] = "Integrated Security=true;Database=Quartz;MultipleActiveResultSets=True;Application Name=Quartz;",
                ["quartz.dataSource.default.provider"] = "SqlServer-20"
            };
            var stdSchedulerFactory = new StdSchedulerFactory(properties);
            stdSchedulerFactory.Initialize();
            _scheduler = stdSchedulerFactory.GetScheduler();
            
        }

        protected override bool Receive(object message)
        {
            return message.Match().With<AddTask>(AddTask).With<RemoveTask>(RemoveTask).WasHandled;
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