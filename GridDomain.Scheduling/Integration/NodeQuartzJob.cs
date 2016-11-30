using System;
using System.IO;
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
    public class NodeQuartzJob : IJob
    {
        private const string CommandKey = nameof(CommandKey);
        private const string EventKey = nameof(EventKey);
        private const string ScheduleKey = nameof(ScheduleKey);
        private const string ExecutionOptionsKey = nameof(ExecutionOptionsKey);

        private readonly IQuartzLogger _quartzLogger;
        private readonly ActorSystem _actorSystem;
        private readonly IPublisher _publisher;
        private readonly IMessageWaiterFactory _executor;
        private readonly WireJsonSerializer _wireJsonSerializer = new WireJsonSerializer();


        public NodeQuartzJob(IQuartzLogger quartzLogger,
                             IMessageWaiterFactory executor)
        {
            _executor = executor;
            Condition.NotNull(() => quartzLogger);
            Condition.NotNull(() => executor);

            _quartzLogger = quartzLogger;
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
                  //  var key = GetScheduleKey(jobDataMap);
                    var options = GetExecutionOptions(jobDataMap);

                    Predicate<object> isExpected = o => (Guid) o.GetType()
                                                               .GetProperty(options.MessageIdFieldName)
                                                               ?.GetValue(o) == options.SuccessMessageId;

                    var success = _executor.NewCommandWaiter()
                                           .Expect(options.SuccesEventType, o => isExpected(o))
                                           .Create()
                                           .Execute(command)
                                           .Wait(options.Timeout);
                    if (!success) 
                        throw new JobTimeoutException(command, options);
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

        private byte[] Serialize(object source)
        {
            return _wireJsonSerializer.ToBinary(source);
        }

        private T Deserialize<T>(byte[] source)
        {
            return (T)_wireJsonSerializer.FromBinary(source,typeof(T));
        }

        private DomainEvent GetEvent(JobDataMap jobDataMap)
        {
            var bytes = jobDataMap[EventKey] as byte[];
            return Deserialize<DomainEvent>(bytes);
        }

        private Command GetCommand(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[CommandKey] as byte[];
            return Deserialize<Command>(bytes);
        }

        private ScheduleKey GetScheduleKey(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[ScheduleKey] as byte[];
            return Deserialize<ScheduleKey>(bytes);
        }

        private ExtendedExecutionOptions GetExecutionOptions(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[ExecutionOptionsKey] as byte[];
            return Deserialize<ExtendedExecutionOptions>(bytes);
        }

        private static IJobDetail CreateJob(ScheduleKey key, JobDataMap jobDataMap)
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

    public class JobTimeoutException : Exception
    {
        public Command Command { get; }
        public ExtendedExecutionOptions Options { get;}

        public JobTimeoutException(Command command, ExtendedExecutionOptions options)
        {
            Command = command;
            Options = options;
        }
    }
}