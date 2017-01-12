using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandWaiter<TCommand> : LocalMessagesWaiter<Task<IWaitResults>>,
                                           ICommandWaiter where TCommand : ICommand
    {
        private readonly CommandExpectBuilder<TCommand> _expectBuilder;

        public CommandWaiter(TCommand command,
                             IMessageMetadata commandMetadata,
                             ActorSystem system, 
                             IActorTransport transport,
                             TimeSpan defaultTimeout):base(system,transport,defaultTimeout)
        {
            _expectBuilder = new CommandExpectBuilder<TCommand>(command, commandMetadata, transport, this);
        }

        public override IExpectBuilder<Task<IWaitResults>> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
             _expectBuilder.And(filter);
            return _expectBuilder;
        }

        public override IExpectBuilder<Task<IWaitResults>> Expect(Type type, Func<object, bool> filter = null)
        {
             _expectBuilder.And(type, filter ?? (o => true));
            return _expectBuilder;
        }

        public Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            return _expectBuilder.Execute(timeout,failOnAnyFault);
        }

        ICommandExpectBuilder ICommandWaiter.Expect<TMsg>(Predicate<TMsg> filter)
        {
            return _expectBuilder.And(filter);
        }

        ICommandExpectBuilder ICommandWaiter.Expect(Type type, Func<object, bool> filter)
        {
          return _expectBuilder.And(type, filter);
        }
    }
}