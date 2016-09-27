using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class NodeCommandExecutor : IGridDomainNode
    {
        private readonly IActorRef _nodeController;
        private readonly TimeSpan _defaultCommandTimeout;

        public NodeCommandExecutor(IActorRef nodeController, TimeSpan defaultCommandTimeout)
        {
            _defaultCommandTimeout = defaultCommandTimeout;
            _nodeController = nodeController;
        }

        public void Execute(params ICommand[] commands)
        {
            foreach (var cmd in commands)
                _nodeController.Tell(cmd);
        }

        public Task<object> Execute(ICommand command, ExpectedMessage[] expectedMessage, TimeSpan? timeout = null)
        {
            var maxWaitTime = timeout ?? _defaultCommandTimeout;
            return _nodeController.Ask(new CommandPlan(command, expectedMessage), maxWaitTime)
                .ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        throw new TimeoutException("Command execution timed out");

                    object result = null;
                    t.Result.Match()
                        .With<IFault>(fault =>
                        {
                            var domainExcpetion = fault.Exception.UnwrapSingle();
                            ExceptionDispatchInfo.Capture(domainExcpetion).Throw();
                        })
                        .With<CommandExecutionFinished>(finish => result = finish.ResultMessage)
                        .Default(m => { throw new InvalidMessageException(m.ToPropsString()); });
                    return result;
                });
        }
    }
}