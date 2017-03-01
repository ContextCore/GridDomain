using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandExpectBuilder<TCommand> : ExpectBuilder<Task<IWaitResults>>,
                                                  ICommandExpectBuilder where TCommand : ICommand
    {
        private readonly TCommand _command;
        private readonly IMessageMetadata _commandMetadata;
        private readonly IActorRef _executorActorRef;
        private readonly LocalMessagesWaiter<Task<IWaitResults>> _waiter;

        public CommandExpectBuilder(TCommand command,
                                    IMessageMetadata commandMetadata,
                                    IActorRef executorActorRef,
                                    LocalMessagesWaiter<Task<IWaitResults>> waiter) : base(waiter)
        {
            _commandMetadata = commandMetadata;
            _executorActorRef = executorActorRef;
            _waiter = waiter;
            _command = command;
        }

        public async Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            Or<IMessageMetadataEnvelop<IFault<TCommand>>>(f => f.Message.Message.Id == _command.Id);
            // Or<IMessageMetadataEnvelop<Fault<TCommand>>>(f => f.Message.Message.Id == _command.Id);

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

        public new ICommandExpectBuilder And<TMsg>(Predicate<TMsg> filter = null)
        {
            if (filter != null)
                base.And(filter);
            else
                base.And<IMessageMetadataEnvelop<TMsg>>(CorrelationFilter);

            return this;
        }

        public new ICommandExpectBuilder Or<TMsg>(Predicate<TMsg> filter = null)
        {
            if (filter != null)
                base.Or(filter);
            else
                base.Or<IMessageMetadataEnvelop<TMsg>>(CorrelationFilter);

            return this;
        }

        private bool CorrelationFilter<T>(IMessageMetadataEnvelop<T> envelop)
        {
            return CorrelationFilter((IMessageMetadataEnvelop) envelop);
        }

        private bool CorrelationFilter(IMessageMetadataEnvelop envelop)
        {
            return envelop?.Metadata?.CorrelationId == _commandMetadata.CorrelationId;
        }

        public override Task<IWaitResults> Create(TimeSpan? timeout)
        {
            return Execute(timeout);
        }

        public new ICommandExpectBuilder And(Type type, Func<object, bool> filter = null)
        {
            if (filter != null)
            {
                base.And(type, filter);
            }
            else
            {
                var envelopType = MessageMetadataEnvelop.GenericForType(type);
                base.And(envelopType, e => CorrelationFilter(e as IMessageMetadataEnvelop));
            }
            return this;
        }
    }
}