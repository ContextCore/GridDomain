using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandConditionBuilder<TCommand> : MetadataConditionBuilder<Task<IWaitResults>>,
                                                     ICommandConditionBuilder where TCommand : ICommand
    {
        private readonly TCommand _command;
        private readonly IMessageMetadata _commandMetadata;
        private readonly IActorRef _executorActorRef;
        private readonly LocalMessagesWaiter<Task<IWaitResults>> _waiter;

        public CommandConditionBuilder(TCommand command,
                                       IMessageMetadata commandMetadata,
                                       IActorRef executorActorRef)
        {
            _commandMetadata = commandMetadata;
            _executorActorRef = executorActorRef;
            _command = command;
        }

        public async Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            Or<IFault<TCommand>>(f => f.Message.Id == _command.Id);

            var task = _waiter.Start(timeout);

            _executorActorRef.Tell(new MessageMetadataEnvelop<ICommand>(_command, _commandMetadata));

            var res = await task;

            if (!failOnAnyFault)
                return res;
            var faults = res.All.OfType<IMessageMetadataEnvelop>().Select(env => env.Message).OfType<IFault>().ToArray();
            if (faults.Any())
                throw new AggregateException(faults.Select(f => f.Exception));

            return res;
        }

        public new ICommandConditionBuilder And<TMsg>(Predicate<TMsg> filter = null)
        {
            base.And(filter);
            return this;
        }

        public new ICommandConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null)
        {
            base.Or(filter);
            return this;
        }
        protected override bool ApplyDefaultFilter<TMsg>(object message)
        {
            var msg = message as MessageMetadataEnvelop<TMsg>;
            return msg != null && msg.Metadata?.CorrelationId == _commandMetadata.CorrelationId;
        }


    }
}