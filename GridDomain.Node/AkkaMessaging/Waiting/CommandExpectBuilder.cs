using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandExpectBuilder : ExpectBuilder<IExpectedCommandExecutor>
    {
        private readonly ICommandExecutor _executor;
        private readonly bool _failOnAnyFault;
        private readonly AkkaCommandLocalWaiter _waiter;

        public CommandExpectBuilder(ICommandExecutor executor, AkkaCommandLocalWaiter waiter, bool failOnAnyFault) : base(waiter)
        {
            _waiter = waiter;
            _failOnAnyFault = failOnAnyFault;
            _executor = executor;
        }

        public override IExpectedCommandExecutor Create(TimeSpan? timeout)
        {
            return new ExpectedCommandExecutor(_executor, _waiter, _failOnAnyFault);
        }
    }

    public class CommandExpectBuilder<TCommand> : ExpectBuilder<Task<IWaitResults>>, 
                                                  ICommandExpectBuilder
                                                  where TCommand : ICommand
    {
        private readonly TCommand _command;
        private readonly IPublisher _publisher;
        private readonly LocalMessagesWaiter<Task<IWaitResults>> _waiter;
        private readonly IMessageMetadata _commandMetadata;

        public CommandExpectBuilder(TCommand command, 
                                    IMessageMetadata commandMetadata, 
                                    IPublisher publisher,
                                    LocalMessagesWaiter<Task<IWaitResults>> waiter) : base(waiter)
        {
            _commandMetadata = commandMetadata;
            _waiter = waiter;
            _command = command;
            _publisher = publisher;
        }

        private bool CorrelationFilter<T>(IMessageMetadataEnvelop<T> envelop)
        {
            return CorrelationFilter((IMessageMetadataEnvelop)envelop);
        }
        private bool CorrelationFilter(IMessageMetadataEnvelop envelop)
        {
            return envelop?.Metadata?.CorrelationId == _commandMetadata.CorrelationId;
        }

        public override Task<IWaitResults> Create(TimeSpan? timeout)
        {
            return Execute(timeout);
        }

        public async Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            Or<IFault<TCommand>>(f => f.Message.Id == _command.Id);
            Or<Fault<TCommand>>(f => f.Message.Id == _command.Id);
            Or<IMessageMetadataEnvelop<IFault<TCommand>>>(f => f.Message.Message.Id == _command.Id);
            Or<IMessageMetadataEnvelop<Fault<TCommand>>>(f => f.Message.Message.Id == _command.Id);

            var task = _waiter.Start(timeout);

            _publisher.Publish(_command, _commandMetadata);

            var res = await task;

            if (!failOnAnyFault) return res;
            var faults = res.All.OfType<IFault>().ToArray();
            if (faults.Any())
                throw new AggregateException(faults.Select(f => f.Exception));

            return res;
        }

        public new ICommandExpectBuilder And<TMsg>(Predicate<TMsg> filter = null)
        {
            if (filter != null)
            {
                base.And(filter);
            }
            else
            {
                base.And<IMessageMetadataEnvelop<TMsg>>(CorrelationFilter);
            }

            return this;
        }

        public new ICommandExpectBuilder And(Type type, Func<object, bool> filter = null)
        {
            if (filter != null)
            {
                base.And(type,filter);
            }
            else
            {
                var envelopType = MessageMetadataEnvelop.GenericForType(type);
                base.And(envelopType, e => CorrelationFilter(e as IMessageMetadataEnvelop));
            }
            return this;
        }

        public new ICommandExpectBuilder Or<TMsg>(Predicate<TMsg> filter = null)
        {
            if (filter != null)
            {
                base.Or(filter);
            }
            else
            {
                base.Or<IMessageMetadataEnvelop<TMsg>>(CorrelationFilter);
            }

            return this;
        }
    }
}