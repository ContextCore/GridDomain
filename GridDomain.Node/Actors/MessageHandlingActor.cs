using System;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class MessageHandlingActor<TMessage, THandler> : UntypedActor where THandler : IHandler<TMessage>
    {
        private readonly THandler _handler;
        private readonly ISoloLogger _log = LogManager.GetLogger();

        public MessageHandlingActor(THandler handler)
        {
            _handler = handler;
            _log.Trace($"Created message handler actor {GetType()}");
            _actorLogName = GetType().BeautyName();
        }

        protected override void OnReceive(object msg)
        {
            Context.IncrementMessagesReceived();
            _log.Trace($"Handler actor got message: {msg.ToPropsString()}");
            _handler.Handle((TMessage)msg);
        }

        private readonly string _actorLogName;

        protected override void PreStart()
        {
            Context.IncrementCounter($"{_actorLogName}.{CounterNames.ActorsCreated}");
        }

        protected override void PostStop()
        {
            Context.IncrementCounter($"{_actorLogName}.{CounterNames.ActorsStopped}");
        }
        protected override void PreRestart(Exception reason, object message)
        {
            Context.IncrementCounter($"{_actorLogName}.{CounterNames.ActorRestarts}");
        }
    }
}