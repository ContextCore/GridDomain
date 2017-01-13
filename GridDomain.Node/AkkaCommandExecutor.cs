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
        public IActorTransport Transport { get; }
        public TimeSpan DefaultTimeout { get; }
        public ActorSystem System { get; }

        public AkkaCommandExecutor(ActorSystem system, IActorTransport transport, TimeSpan defaultTimeout)
        {
            Transport = transport;
            System = system;
            DefaultTimeout = defaultTimeout;
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
               Transport.Publish(command, metadata);
        }


        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            var commandMetadata = metadata ?? new MessageMetadata(cmd.Id,
                                                                  BusinessDateTime.UtcNow,
                                                                  Guid.NewGuid(),
                                                                  Guid.Empty,
                                                                  new ProcessHistory(new[] {
                                                                    new ProcessEntry(nameof(AkkaCommandExecutor),
                                                                                  "publishing command to transport",
                                                                                  "command is executing")}));

            return new CommandWaiter<T>(cmd, commandMetadata, System, Transport, DefaultTimeout);
        }
    }
}