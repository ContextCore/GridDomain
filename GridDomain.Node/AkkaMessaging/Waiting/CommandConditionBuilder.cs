using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{


    public class CommandConditionBuilderTypedDecorator<T> : ICommandConditionBuilder<T>
    {
        private readonly ICommandConditionBuilder _commandConditionBuilder;

        public CommandConditionBuilderTypedDecorator(ICommandConditionBuilder builder)
        {
            _commandConditionBuilder = builder;
        }

        Task<IWaitResult> ICommandConditionBuilder.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            return _commandConditionBuilder.Execute(timeout, failOnAnyFault);
        }

        public ICommandConditionBuilder And<TMsg>(Predicate<TMsg> filter = null)
        {
            return _commandConditionBuilder.And(filter);

        }

        public ICommandConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null)
        {
            return _commandConditionBuilder.Or(filter);

        }

        async Task<IWaitResult<T>> ICommandConditionBuilder<T>.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            var res = await _commandConditionBuilder.Execute();
            return new WaitResult<T>(res.All.OfType<IMessageMetadataEnvelop<T>>().First()); 
        }
    }
    public class CommandConditionBuilder<TCommand> : MetadataConditionBuilder<Task<IWaitResult>>,
                                                     ICommandConditionBuilder where TCommand : ICommand
    {
        private readonly TCommand _command;
        private readonly IMessageMetadata _commandMetadata;
        private readonly IActorRef _executorActorRef;

        public CommandConditionBuilder(TCommand command,
                                       IMessageMetadata commandMetadata,
                                       IActorRef executorActorRef)
        {
            _commandMetadata = commandMetadata;
            _executorActorRef = executorActorRef;
            _command = command;
        }

        public async Task<IWaitResult> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            //we can launch any command with CommandConditionBuilder<ICommand>
            //and TCommand will resolve to ICommand leading to incorect subscription
            Or(Fault.TypeFor(_command), IsFaultFromExecutingCommand);

            var task = Create(timeout);

            _executorActorRef.Tell(new MessageMetadataEnvelop<ICommand>(_command, _commandMetadata));

            var res = await task;

            if (!failOnAnyFault)
                return res;
            var faults = res.All.OfType<IMessageMetadataEnvelop>().Select(env => env.Message).OfType<IFault>().ToArray();
            if (faults.Any())
                throw new AggregateException(faults.Select(f => f.Exception));

            return res;
        }

        private bool IsFaultFromExecutingCommand(object o)
        {
            return (((o as IMessageMetadataEnvelop)?.Message as IFault)?.Message as ICommand)?.Id == _command.Id;
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
        protected override bool DefaultFilter<TMsg>(object received)
        {
            //interface is important as it provides cavoriance
            //it allows to check messages with concete inner types when TMsg is object
            var msg = received as IMessageMetadataEnvelop<TMsg>;
            return msg != null && msg.Metadata?.CorrelationId == _commandMetadata.CorrelationId;
        }


    }
}