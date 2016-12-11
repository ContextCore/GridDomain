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
using Quartz.Impl.Triggers;
using Wire;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJob : IJob
    {
        public const string CommandKey = nameof(CommandKey);
        public const string EventKey = nameof(EventKey);
        public const string ScheduleKey = nameof(ScheduleKey);
        public const string ExecutionOptionsKey = nameof(ExecutionOptionsKey);

        private readonly IQuartzLogger _quartzLogger;
        private readonly IPublisher _publisher;
        private readonly IMessageWaiterFactory _executor;
        private readonly WireJsonSerializer _serializer = new WireJsonSerializer();


        public QuartzJob(IQuartzLogger quartzLogger,
                         IPublisher publisher,
                         IMessageWaiterFactory executor)
        {
            Condition.NotNull(()=> quartzLogger);
            Condition.NotNull(()=> publisher);

            _executor = executor;
            _quartzLogger = quartzLogger;
            _publisher = publisher;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                ExtendedExecutionOptions extendedOptions;
                var jobDataMap = context.JobDetail.JobDataMap;
                if (jobDataMap.ContainsKey(CommandKey))
                {
                    var command = GetCommand(jobDataMap);
                    var key = GetScheduleKey(jobDataMap);
                    var options = GetExecutionOptions(jobDataMap, context.JobDetail.Key.Name);
                    if (options.SuccesEventType == null)
                        throw new Exception("options do not have SuccessEventType for key " + key);

                    Predicate<object> isExpected = (o => true);
                    //we should work with legacy jobs having only ExecutionOptions, not ExtendedExecutionOptions 
                    extendedOptions = options as ExtendedExecutionOptions;

                    if (!string.IsNullOrEmpty(extendedOptions?.MessageIdFieldName))
                    {
                        isExpected = o => o.GetType()
                                           .IsAssignableFrom(extendedOptions.SuccesEventType)
                                    && (Guid)o?.GetType()
                                           .GetProperty(extendedOptions.MessageIdFieldName)?
                                           .GetValue(o) == extendedOptions.SuccessMessageId;
                    }
                    else
                        _quartzLogger.LogWarn(context.JobDetail.Key.Name, "Received extended options with empty id property field");

                    var task = _executor.NewCommandWaiter(options.Timeout)
                                        .Expect(options.SuccesEventType, o => isExpected(o))
                                        .Create()
                                        .Execute(command);

                    if (!task.Wait(options.Timeout))
                    {
                        throw new ScheduledCommandWasNotConfirmedException(command);
                    }

                    _quartzLogger.LogSuccess(context.JobDetail.Key.Name);
                    _publisher.Publish(new JobSucceeded(context.JobDetail.Key.Name, context.JobDetail.Key.Group));
                }
                else
                {
                    var messageToFire = GetEvent(jobDataMap);
                    _publisher.Publish(messageToFire);
                    _publisher.Publish(new JobSucceeded(context.JobDetail.Key.Name, context.JobDetail.Key.Group));
                }
            }
            catch (Exception e)
            {
                _quartzLogger.LogFailure(context.JobDetail.Key.Name, e);
                _publisher.Publish(new JobFailed(context.JobDetail.Key.Name, context.JobDetail.Key.Group, e));
                throw new JobExecutionException(e, false);
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

        private static byte[] Serialize(object source, WireJsonSerializer serializer = null)
        {
            return (serializer ?? new WireJsonSerializer()).ToBinary(source);
        }

        private static T Deserialize<T>(byte[] source, WireJsonSerializer serializer = null)
        {
            return (T)(serializer ?? new WireJsonSerializer()).FromBinary(source, typeof(T));
        }

        private DomainEvent GetEvent(JobDataMap jobDataMap)
        {
            var bytes = jobDataMap[EventKey] as byte[];
            return Deserialize<DomainEvent>(bytes, _serializer);
        }

        private Command GetCommand(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[CommandKey] as byte[];
            return Deserialize<Command>(bytes, _serializer);
        }

        private ScheduleKey GetScheduleKey(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[ScheduleKey] as byte[];
            return Deserialize<ScheduleKey>(bytes, _serializer);
        }

        private ExecutionOptions GetExecutionOptions(JobDataMap jobDatMap, string jobName)
        {
            var bytes = jobDatMap[ExecutionOptionsKey] as byte[];
            try
            {
                return Deserialize<ExtendedExecutionOptions>(bytes);
            }
            catch (Exception ex)
            {
                _quartzLogger.LogWarn(jobName, $"Cannot deserialize extended options, will switch to simple options. {ex}");
                return Deserialize<ExecutionOptions>(bytes);
            }
        }

        public static IJobDetail CreateJob(ScheduleKey key, JobDataMap jobDataMap)
        {
            var jobKey = new JobKey(key.Name, key.Group);
            return JobBuilder.Create<QuartzJob>()
                             .WithIdentity(jobKey)
                             .WithDescription(key.Description)
                             .UsingJobData(jobDataMap)
                             .RequestRecovery(true)
                             .Build();
        }
    }
}