using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz.Logging;
using Newtonsoft.Json;
using Quartz;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJob : IJob
    {
        private const string CommandKey = nameof(CommandKey);
        private const string EventKey = nameof(EventKey);
        private const string ScheduleKey = nameof(ScheduleKey);
        private const string ExecutionOptionsKey = nameof(ExecutionOptionsKey);

        private readonly IQuartzLogger _quartzLogger;
        private readonly ActorSystem _actorSystem;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
        };

        public QuartzJob(
            IQuartzLogger quartzLogger,
            ActorSystem actorSystem
            )
        {
            _quartzLogger = quartzLogger;
            _actorSystem = actorSystem;
        }

        public void Execute(IJobExecutionContext context)
        {
            bool isFirstTimeFiring = true;
            try
            {
                isFirstTimeFiring = context.RefireCount == 0;
                if (context.JobDetail.JobDataMap.ContainsKey(CommandKey))
                {
                    var command = GetCommand(context.JobDetail.JobDataMap);
                    var key = GetScheduleKey(context.JobDetail.JobDataMap);
                    var options = GetExecutionOptions(context.JobDetail.JobDataMap);
                    var sagaCreator = _actorSystem.ActorOf(CreateGenericProps(options));
                    var result = sagaCreator.Ask(new ManageScheduledCommand(command, key), options.Timeout);
                    result.Wait(options.Timeout);
                }
                else
                {
                    var eventToFire = 
                }
            }
            catch (JsonSerializationException e)
            {
                _quartzLogger.LogFailure(context.JobDetail.Key.Name, e);
            }
            catch (Exception e)
            {
                _quartzLogger.LogFailure(context.JobDetail.Key.Name, e);
                var jobExecutionException = new JobExecutionException(e) { RefireImmediately = isFirstTimeFiring };
                throw jobExecutionException;
            }
        }

        public static JobBuilder Create(ScheduleKey key, Command command, ExecutionOptions executionOptions)
        {
            var serializedCommand = Serialize(command);
            var serializedKey = Serialize(key);
            var serializedOptions = Serialize(executionOptions);

            var jobDataMap = new JobDataMap
            {
                { CommandKey, serializedCommand },
                { ScheduleKey, serializedKey },
                { ExecutionOptionsKey, serializedOptions }
            };
            return CreateJobBuilder(key, jobDataMap);
        }

        public static JobBuilder Create(ScheduleKey key, DomainEvent eventToSchedule)
        {
            var serializedEvent = Serialize(eventToSchedule);
            var serializedKey = Serialize(key); var jobDataMap = new JobDataMap
            {
                { EventKey, serializedEvent },
                { ScheduleKey, serializedKey }
            };
            return CreateJobBuilder(key, jobDataMap);
        }

        private Props CreateGenericProps(ExecutionOptions options)
        {
            var genericActorType = typeof(ScheduledSagaCreator<>).MakeGenericType(options.SuccessEventType);
            return _actorSystem.DI().Props(genericActorType);
        }

        private static DomainEvent GetEvent(JobDataMap jobDataMap)
        {
            
        }

        private static Command GetCommand(JobDataMap jobDatMap)
        {
            var json = jobDatMap[CommandKey] as string;
            var command = Deserialize<Command>(json);
            return command;
        }

        private static ScheduleKey GetScheduleKey(JobDataMap jobDatMap)
        {
            var json = jobDatMap[ScheduleKey] as string;
            var scheduleKey = Deserialize<ScheduleKey>(json);
            return scheduleKey;
        }

        private static ExecutionOptions GetExecutionOptions(JobDataMap jobDatMap)
        {
            var json = jobDatMap[ExecutionOptionsKey] as string;
            var executionOptions = Deserialize<ExecutionOptions>(json);
            return executionOptions;
        }

        private static JobBuilder CreateJobBuilder(ScheduleKey key, JobDataMap jobDataMap)
        {
            var jobKey = new JobKey(key.Name, key.Group);
            return JobBuilder
                .Create<QuartzJob>()
                .WithIdentity(jobKey)
                .WithDescription(key.Description)
                .UsingJobData(jobDataMap)
                .RequestRecovery(true);
        }

        private static string Serialize(object source)
        {
            return JsonConvert.SerializeObject(source, JsonSerializerSettings);
        }

        private static T Deserialize<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source, JsonSerializerSettings);
        }
    }
}