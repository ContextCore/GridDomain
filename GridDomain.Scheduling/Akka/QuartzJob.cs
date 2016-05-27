using System;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Tasks;
using Newtonsoft.Json;
using Quartz;

namespace GridDomain.Scheduling.Akka
{
    public class QuartzJob : IJob
    {
        private readonly ITargetActorProvider _actorProvider;

        public QuartzJob(ITargetActorProvider actorProvider)
        {
            _actorProvider = actorProvider;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var taskJson = context.JobDetail.JobDataMap["task"] as string;
                var task = JsonConvert.DeserializeObject<AkkaScheduledTask>(taskJson,new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                var targetActor = _actorProvider.Get(task.Request);
                targetActor.Ask(task.Request, task.Timeout).Wait(task.Timeout);
            }
            catch (Exception e)
            {
                throw new JobExecutionException(e);
            }

        }

        public static JobBuilder Create(AkkaScheduledTask task)
        {
            var serialized = JsonConvert.SerializeObject(task);
            var jdm = new JobDataMap { { "task", serialized } };
            return JobBuilder.Create<QuartzJob>().WithIdentity(task.Request.TaskId).UsingJobData(jdm);
        }
    }
}