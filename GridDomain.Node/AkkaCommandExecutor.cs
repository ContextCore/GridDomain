using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Executes commands. Should not be used inside actors
    /// </summary>
    public class AkkaCommandExecutor : ICommandExecutor
    {
        private readonly IActorTransport _transport;
        private readonly ActorSystem _system;

        public AkkaCommandExecutor(ActorSystem system, IActorTransport transport)
        {
            _system = system;
            _transport = transport;
        }

        public void Execute(params ICommand[] commands)
        {
            foreach (var cmd in commands)
                _transport.Publish(cmd);
        }

        public Task<object> Execute(CommandPlan plan)
        {
          //  var commandWaiter = new CommandMessageWaiter(this, _system, plan.Timeout);
            
            var waiter = new AkkaMessageLocalWaiter(_system,_transport);
            
            //var expectBuilder = waiter.Expect();

            foreach (var expectedMessage in plan.ExpectedMessages)
            {
            }

            Execute(plan.Command);
            throw new NotImplementedException();
          // return commandWaiter.WaitFor(plan);
        }

       

        public Task<T> Execute<T>(CommandPlan<T> plan)
        {
            return Execute((CommandPlan)plan).ContinueWithSafeResultCast(result => (T)result);
        }
    }
}