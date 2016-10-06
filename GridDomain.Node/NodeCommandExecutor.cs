using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class NodeCommandExecutor : ICommandExecutor
    {
        private readonly IActorRef _nodeController;
        private ISoloLogger _logger = LogManager.GetLogger();
        public NodeCommandExecutor(IActorRef nodeController)
        {
            _nodeController = nodeController;
        }

        public void Execute(params ICommand[] commands)
        {
            foreach (var cmd in commands)
                _nodeController.Tell(cmd);
        }

        public Task<object> Execute(CommandPlan plan)
        {
            return _nodeController.Ask(plan, plan.Timeout)
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
                                          .With<Failure>(f =>
                                          {
                                              if(f.Exception is TimeoutException)
                                                  throw new TimeoutException("Command execution timed out");
                                          })
                                          .With<CommandExecutionFinished>(finish => result = finish.ResultMessage)
                                          .Default(m =>
                                          {
                                              var invalidMessageException = new InvalidMessageException();
                                              _logger.Error(invalidMessageException,"Received unexpected message while waiting for command execution: {Message}",m);
                                              throw invalidMessageException;
                                          });
                                      return result;
                                  });
        }

        public Task<T> Execute<T>(CommandPlan<T> plan)
        {
            return Execute((CommandPlan)plan).ContinueWithSafeResultCast(result => (T)result);
        }
    }
}