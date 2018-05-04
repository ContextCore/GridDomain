using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandEventsFilter<TCommand> : ICommandEventsFilter where TCommand : ICommand
    {
        private readonly TCommand _command;
        private readonly IMessageMetadata _commandMetadata;
        private readonly ICommandExecutor _executorActorRef;
        public readonly MessageConditionFactory<Task<IWaitResult>> MessageConditionFactory;

        public CommandEventsFilter(TCommand command,
                                   IMessageMetadata commandMetadata,
                                   ICommandExecutor executorActorRef,
                                   MessageConditionFactory<Task<IWaitResult>> messageConditionFactory = null)
        {
            MessageConditionFactory = messageConditionFactory ?? new MessageConditionFactory<Task<IWaitResult>>(new LocalCorrelationConditionFactory(commandMetadata.CorrelationId));
            _commandMetadata = commandMetadata;
            _executorActorRef = executorActorRef;
            _command = command;
        }

        public async Task<IWaitResult> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            //we can launch any command with CommandConditionBuilder<ICommand>
            //and TCommand will resolve to ICommand leading to incorect subscription
            if (failOnAnyFault)
                MessageConditionFactory.Or(Fault.TypeFor(_command), IsFaultFromExecutingCommand);

            var task = MessageConditionFactory.Create(timeout);

            //will wait later in task; 
#pragma warning disable 4014
            _executorActorRef.Execute(_command, _commandMetadata, CommandConfirmationMode.None);
#pragma warning restore 4014

            var res = await task;

            if (!failOnAnyFault)
                return res;
            var faults = res.All.OfType<IMessageMetadataEnvelop>()
                            .Select(env => env.Message)
                            .OfType<IFault>()
                            .ToArray();
            if (faults.Any())
                throw new AggregateException(faults.Select(f => f.Exception));

            return res;
        }

        private bool IsFaultFromExecutingCommand(object o)
        {
            return (((o as IMessageMetadataEnvelop)?.Message as IFault)?.Message as ICommand)?.Id == _command.Id;
        }

        public ICommandEventsFilter And<TMsg>(Predicate<TMsg> filter) where TMsg : class
        {
            MessageConditionFactory.And(filter);
            return this;
        }

        public ICommandEventsFilter Or<TMsg>(Predicate<TMsg> filter) where TMsg : class
        {
            MessageConditionFactory.Or(filter);
            return this;
        }

    }
}