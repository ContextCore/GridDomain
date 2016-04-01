using System;
using Akka.Actor;
using NLog;

namespace GridDomain.Node.Actors
{
    internal class CommandHandlerActor : UntypedActor
    {
        public readonly Guid Id = Guid.NewGuid();
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        protected override void OnReceive(object message)
        {
            log.Info($"Актор {GetType().Name} id={Id} получил сообщение:\r\n{message}");
        }
    }
}