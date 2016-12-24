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
        public const string MetadataKey = nameof(MetadataKey);
        public const string PassingCommandToExecutor = "passing command to executor";
        public const string CommandRaiseTimeCame = "command raise time came";
        public const string PublishingEvent = "publishing event";
        public const string EventRaiseTimeCame = "event raise time came";
        public const string PublishingJobFaulire = "publishing job faulire";
        public const string JobRaiseTimeCame = "job raise time came";

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
            var jobDataMap = context.JobDetail.JobDataMap;
            var metadata = SafeGetMetadata(jobDataMap);
            var jobKey = context.JobDetail.Key;

            if (jobDataMap.ContainsKey(CommandKey))
            {
                var command = GetCommand(jobDataMap);
                WithErrorHandling(command, metadata, jobKey, () => ProcessCommand(command,jobDataMap,metadata,jobKey));
            }
            else
            {
                var evt = GetEvent(jobDataMap);
                WithErrorHandling(evt, metadata, jobKey, () => ProcessEvent(metadata, jobKey, evt));
            }
        }

        private void ProcessEvent(IMessageMetadata metadata, JobKey jobKey, DomainEvent messageToFire)
        {
            var eventMetadata = metadata.CreateChild(messageToFire.SourceId,
                                                     new ProcessEntry(nameof(QuartzJob),
                                                                      PublishingEvent,
                                                                      EventRaiseTimeCame));

            var successMetadata = eventMetadata.CreateChild(Guid.NewGuid(),
                                            new ProcessEntry(nameof(QuartzJob), 
                                                             "Publishing success notification",
                                                             "Job execution completed succesfully. Message published."));

            var jobSucceeded = new JobSucceeded(jobKey.Name,
                                                jobKey.Group,
                                                messageToFire);

            _publisher.Publish(messageToFire, eventMetadata);
            _publisher.Publish(jobSucceeded, successMetadata);
        }

        private void WithErrorHandling(object processingMessage, IMessageMetadata messageMetadata, JobKey key, Action act)
        {
            try
            {
                act();
            }
            catch (Exception e)
            {
                _quartzLogger.LogFailure(key.Name, e);
                var jobFailed = new JobFailed(key.Name, key.Group, e, processingMessage);
                var jobFailedMetadata = messageMetadata.CreateChild(Guid.Empty,
                                                                    new ProcessEntry(nameof(QuartzJob),
                                                                                     PublishingJobFaulire,
                                                                                     JobRaiseTimeCame));
                _publisher.Publish(jobFailed, jobFailedMetadata);
                throw new JobExecutionException(e, false);
            }
        }

        private void ProcessCommand(ICommand command, JobDataMap jobDataMap, IMessageMetadata metadata, JobKey jobKey)
        {
                var key = GetScheduleKey(jobDataMap);
                var options = GetExecutionOptions(jobDataMap, jobKey.Name);
                if (options.SuccesEventType == null)
                    throw new Exception("options do not have SuccessEventType for key " + key);

                Predicate<object> isExpected = (o => true);
                //we should work with legacy jobs having only ExecutionOptions, not ExtendedExecutionOptions 
                var extendedOptions = options as ExtendedExecutionOptions;

                if (!string.IsNullOrEmpty(extendedOptions?.MessageIdFieldName))
                {
                    isExpected = o =>
                    {
                      var envelop = o as IMessageMetadataEnvelop;
                      if (envelop != null) o = envelop.Message;

                      //received fault instead of expected message
                       var type = o?.GetType();
                       var guid = type?.GetProperty(extendedOptions.MessageIdFieldName)?
                                        .GetValue(o);

                      return guid != null && 
                       (Guid)guid == extendedOptions.SuccessMessageId;
                    };
                }

                var commandMetadata = metadata.CreateChild(command.Id,
                                                           new ProcessEntry(nameof(QuartzJob),
                                                                            PassingCommandToExecutor,
                                                                            CommandRaiseTimeCame));

                var task = _executor.NewCommandWaiter(options.Timeout)
                                    .Expect(options.SuccesEventType, o => isExpected(o))
                                    .Create()
                                    .Execute(command, commandMetadata);

                if (!task.Wait(options.Timeout))
                    throw new ScheduledCommandWasNotConfirmedException(command);

                _quartzLogger.LogSuccess(jobKey.Name);

               var successMetadata = commandMetadata.CreateChild(Guid.NewGuid(),
                                                 new ProcessEntry(nameof(QuartzJob), "Publishing success notification",
                                                     "Job execution completed succesfully. Command executed and confirmed."));

                var jobSucceeded = new JobSucceeded(jobKey.Name,
                                                    jobKey.Group,
                                                    command);

                _publisher.Publish(jobSucceeded, successMetadata);
        }

        private IMessageMetadata SafeGetMetadata(JobDataMap jobDataMap)
        {
            var bytes = jobDataMap[MetadataKey] as byte[];
            try
            {
                return Deserialize<IMessageMetadata>(bytes, _serializer);
            }
            catch (Exception ex)
            {
                return MessageMetadata.Empty();
            }
        }

        public static IJobDetail Create(ScheduleKey key, Command command, IMessageMetadata metadata, ExecutionOptions executionOptions)
        {
            var serializedCommand = Serialize(command);
            var serializedKey = Serialize(key);
            var serializedOptions = Serialize(executionOptions);
            var serializedMetadata = Serialize(metadata);

            var jobDataMap = new JobDataMap
            {
                { CommandKey, serializedCommand },
                { ScheduleKey, serializedKey },
                { ExecutionOptionsKey, serializedOptions },
                { MetadataKey, serializedMetadata }
            };
            return CreateJob(key, jobDataMap);
        }

        public static IJobDetail Create(ScheduleKey key, DomainEvent eventToSchedule, IMessageMetadata metadata)
        {
            var serializedEvent = Serialize(eventToSchedule);
            var serializedKey = Serialize(key);
            var serializedMetadata = Serialize(metadata);

            var jobDataMap = new JobDataMap
            {
                { EventKey, serializedEvent },
                { ScheduleKey, serializedKey },
                { MetadataKey, serializedMetadata }
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
                _quartzLogger.LogFailure(jobName, ex);
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