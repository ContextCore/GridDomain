using System;
using System.Runtime.ExceptionServices;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Akka.Tasks;
using GridDomain.Scheduling.Quartz.Logging;
using Newtonsoft.Json;
using Quartz;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJob : IJob
    {
        private const string TaskKey = "Task";
        private const string Timeout = "Timeout";
        private readonly ITaskRouter _taskRouter;
        private readonly IQuartzLogger _logger;
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
        };

        public QuartzJob()
        {
            _taskRouter = TaskRouterFactory.Get();
            _logger = QuartzLoggerFactory.GetLogger();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var scheduledRequest = DeserializeTaskData(context.JobDetail.JobDataMap);
                var timeout = DeserializeTimeout(context.JobDetail.JobDataMap);
                var targetActor = _taskRouter.GetTarget(scheduledRequest);
                //TODO::VZ:: is there a better way to communicate with akka?
                var result = targetActor.Ask(scheduledRequest, timeout);
                result.Wait(timeout);
                //TODO::VZ refactor without casts
                var success = result.Result as TaskProcessed;
                if (success != null)
                {
                    _logger.LogSuccess(context.JobDetail.Key.Name);
                }
                else
                {
                    var failure = result.Result as Failure;
                    if (failure != null)
                    {
                        ExceptionDispatchInfo.Capture(failure.Exception).Throw();
                    }
                    throw new InvalidOperationException($"Wrong reply from task handler actor. Reply: ${result.Result}");
                }
            }
            catch (Exception e)
            {
                _logger.LogFailure(context.JobDetail.Key.Name, e);
                throw new JobExecutionException(e);
            }
        }

        private static ScheduledRequest DeserializeTaskData(JobDataMap jobDatMap)
        {
            var taskJson = jobDatMap[TaskKey] as string;
            //TODO::VZ:: use external wrapper around serializer?
            var task = JsonConvert.DeserializeObject<ScheduledRequest>(taskJson, JsonSerializerSettings);
            return task;
        }

        private static TimeSpan DeserializeTimeout(JobDataMap jobDatMap)
        {
            var timeout = TimeSpan.Parse(jobDatMap[Timeout] as string);
            return timeout;
        }

        public static JobBuilder Create(ScheduledRequest task, TimeSpan timeout)
        {
            //TODO::VZ:: use external wrapper around serializer?
            var serialized = JsonConvert.SerializeObject(task, JsonSerializerSettings);
            var jdm = new JobDataMap { { TaskKey, serialized }, { Timeout, timeout.ToString() } };
            return JobBuilder.Create<QuartzJob>().WithIdentity(task.TaskId).UsingJobData(jdm);
        }
    }
}