using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
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

        public AkkaCommandExecutor(IActorTransport transport)
        {
            _transport = transport;
        }

        public void Execute(params ICommand[] commands)
        {
            foreach (var cmd in commands)
            {
                var metadata = MessageMetadata.Empty()
                                              .CreateChild(cmd.Id, new ProcessEntry(nameof(AkkaCommandExecutor),
                                                                                    "publishing command to transport",
                                                                                    "command is executing"));
                Execute(cmd, metadata);
            }
        }

        public void Execute<T>(T command, IMessageMetadata metadata) where T : ICommand
        {
               _transport.Publish(command, metadata);
        }
    }
}