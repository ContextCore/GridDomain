using System;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Tasks;
using Newtonsoft.Json;
using Quartz;

namespace GridDomain.Scheduling.Akka
{
    public class QuartzJob : IJob
    {
        private readonly ITaskRouter _taskRouter;

        public QuartzJob()
        {
            _taskRouter = TaskRouterFactory.Get();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var taskJson = context.JobDetail.JobDataMap["task"] as string;
                var task = JsonConvert.DeserializeObject<AkkaScheduledTask>(taskJson, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
                var targetActor = _taskRouter.GetTarget(task.Request);
                targetActor.Ask(task.Request, task.Timeout).Wait(task.Timeout);
            }
            catch (Exception e)
            {
                throw new JobExecutionException(e);
            }

        }

        public static JobBuilder Create(AkkaScheduledTask task)
        {
            var serialized = JsonConvert.SerializeObject(task, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });

            var jdm = new JobDataMap { { "task", serialized } };
            return JobBuilder.Create<QuartzJob>().WithIdentity(task.Request.TaskId).UsingJobData(jdm);
        }
    }
}