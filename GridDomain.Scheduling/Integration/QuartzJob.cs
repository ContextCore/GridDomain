using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz.Logging;
using Quartz;
using Wire;

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
        private readonly IPublisher _publisher;
        private static readonly WireJsonSerializer _serializer = new WireJsonSerializer();


        public QuartzJob(IQuartzLogger quartzLogger,
                         ActorSystem actorSystem,
                         IPublisher publisher)
        {
            Condition.NotNull(()=> quartzLogger);
            Condition.NotNull(()=> actorSystem);
            Condition.NotNull(()=> publisher);
            Condition.NotNull(() => actorSystem.DI());

            _quartzLogger = quartzLogger;
            _actorSystem = actorSystem;
            _publisher = publisher;
        }

        public void Execute(IJobExecutionContext context)
        {
            bool isFirstTimeFiring = true;
            try
            {
                isFirstTimeFiring = context.RefireCount == 0;
                var jobDataMap = context.JobDetail.JobDataMap;
                if (jobDataMap.ContainsKey(CommandKey))
                {
                    var command = GetCommand(jobDataMap);
                    var key = GetScheduleKey(jobDataMap);
                    var options = GetExecutionOptions(jobDataMap);
                    var genericProps = CreateGenericProps(options);
                    var sagaCreator = _actorSystem.ActorOf(genericProps);
                    var result = sagaCreator.Ask(new StartSchedulerSaga(command, key), options.Timeout);
                    var r = result.Wait(options.Timeout);
                    if(!r) throw new Exception("Scheduling saga was not created");
                    sagaCreator.Tell(PoisonPill.Instance, ActorRefs.NoSender);
                }
                else
                {
                    var messageToFire = GetEvent(jobDataMap);
                    _publisher.Publish(messageToFire);
                }
            }
            catch (Exception e)
            {
                _quartzLogger.LogFailure(context.JobDetail.Key.Name, e);
                var jobExecutionException = new JobExecutionException(e) { RefireImmediately = isFirstTimeFiring };
                throw jobExecutionException;
            }
        }

        public static IJobDetail Create(ScheduleKey key, Command command, ExecutionOptions executionOptions)
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
            return CreateJob(key, jobDataMap);
        }

        public static IJobDetail Create(ScheduleKey key, DomainEvent eventToSchedule)
        {
            var serializedEvent = Serialize(eventToSchedule);
            var serializedKey = Serialize(key);

            var jobDataMap = new JobDataMap
            {
                { EventKey, serializedEvent },
                { ScheduleKey, serializedKey }
            };
            return CreateJob(key, jobDataMap);
        }

        private static byte[] Serialize(object source)
        {
            return _serializer.ToBinary(source);
        }

        private static T Deserialize<T>(byte[] source)
        {
            return (T)_serializer.FromBinary(source, typeof(T));
        }

        private Props CreateGenericProps(ExecutionOptions options)
        {
            var genericActorType = typeof(ScheduledSagaCreator<>).MakeGenericType(options.SuccesEventType);
            return _actorSystem.DI().Props(genericActorType);
        }

        private static DomainEvent GetEvent(JobDataMap jobDataMap)
        {
            var bytes = jobDataMap[EventKey] as byte[];
            return Deserialize<DomainEvent>(bytes);
        }

        private static Command GetCommand(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[CommandKey] as byte[];
            return Deserialize<Command>(bytes);
        }

        private static ScheduleKey GetScheduleKey(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[ScheduleKey] as byte[];
            return Deserialize<ScheduleKey>(bytes);
        }

        private static ExecutionOptions GetExecutionOptions(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[ExecutionOptionsKey] as byte[];
            return Deserialize<ExecutionOptions>(bytes);
        }

        public static IJobDetail CreateJob(ScheduleKey key, JobDataMap jobDataMap)
        {
            var jobKey = new JobKey(key.Name, key.Group);
            return JobBuilder
                   .Create<QuartzJob>()
                   .WithIdentity(jobKey)
                   .WithDescription(key.Description)
                   .UsingJobData(jobDataMap)
                   .RequestRecovery(true)
                   .Build();
        }
    }
}