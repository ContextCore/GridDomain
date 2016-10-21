using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Util.Internal;
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
            var waiter = new AkkaMessageLocalWaiter(_system,_transport, TimeSpan.FromSeconds(10));
            
            var expectBuilder = waiter.ExpectBuilder;

            foreach (var expectedMessage in plan.ExpectedMessages.Where(e => !typeof(IFault).IsAssignableFrom(e.MessageType)))
            {
                expectBuilder.And(expectedMessage.MessageType, o => expectedMessage.Match(o));
            }

            foreach (var expectedMessage in plan.ExpectedMessages.Where(e => e.MessageType.IsInstanceOfType(typeof(IFault))))
            {
                expectBuilder.Or(expectedMessage.MessageType,
                                 o => expectedMessage.Match(o) &&
                                      (!expectedMessage.Sources.Any() ||
                                        expectedMessage.Sources.Contains((o as IFault)?.Processor)));
            }

            var commandFaultType = typeof(IFault<>).MakeGenericType(plan.Command.GetType());

            expectBuilder.Or(commandFaultType,
                             o => ((o as IFault)?.Message as ICommand)?.Id == plan.Command.Id);


            var task = expectBuilder.Create(plan.Timeout)
                                    .ContinueWith(t =>
                                    {
                                        CheckTaskFault(t);

                                        var expectedFault = t.Result.Message<IFault>(f => commandFaultType.IsInstanceOfType(f));
                                        if (expectedFault != null)
                                            ExceptionDispatchInfo.Capture(expectedFault.Exception.UnwrapSingle()).Throw();

                                        return t.Result.All.Count > 1 ? t.Result.All.ToArray() : t.Result.All.FirstOrDefault();

                                    });

            Execute(plan.Command);

            return task;
        }

        private static void CheckTaskFault(Task t)
        {
            if (t.IsCanceled)
                throw new TimeoutException();

            if (t.IsFaulted)
                ExceptionDispatchInfo.Capture(t.Exception.UnwrapSingle()).Throw();
        }


        public Task<T> Execute<T>(CommandPlan<T> plan)
        {
            return Execute((CommandPlan)plan).ContinueWith(t =>
            {
                CheckTaskFault(t);
                return (T) t.Result;
            });
        }
    }
}