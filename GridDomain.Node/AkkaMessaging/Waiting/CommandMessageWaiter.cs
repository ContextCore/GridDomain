using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using MemBus.Support;

namespace GridDomain.Node.AkkaMessaging.Waiting
{

    class CommandMessageWaiter : LocalMessageWaiter, ICommandWaiter
    {
        private readonly ICommandExecutor _executor;

        internal CommandMessageWaiter(ICommandExecutor executor,IActorRef receiver, TimeSpan timeout) : base(receiver, timeout)
        {
            _executor = executor;
        }

        public IMessageWaiter Execute(ICommand command)
        {
            _executor.Execute(command);
            return this;
        }

        public Task<object> WaitFor(CommandPlan p)
        {
            throw new NotImplementedException();
        }

        public static CommandMessageWaiter New(ICommandExecutor executor, ActorSystem system, TimeSpan timeout)
        {
            var props = Props.Create(() => new CommandWaiterActor(null));
            var waitActor = system.ActorOf(props, "MessagesWaiter_" + Guid.NewGuid());

            return new CommandMessageWaiter(executor,waitActor, timeout);
        }

        ICommandWaiter ICommandWaiter.Execute(ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}