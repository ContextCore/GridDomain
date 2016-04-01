using System;
using Akka.Actor;
using NLog;

namespace GridDomain.Node.Actors
{
    class CommandHandlerActor : UntypedActor
    {
        public readonly Guid Id = Guid.NewGuid();
        private Logger log = LogManager.GetCurrentClassLogger();
        protected override void OnReceive(object message)
        {
            log.Info($"Актор {this.GetType().Name} id={this.Id} получил сообщение:\r\n{message}");
        }
    }
}