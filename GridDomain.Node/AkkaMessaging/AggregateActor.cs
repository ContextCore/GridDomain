using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging
{
    /// <summary>
    /// Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public abstract class AggregateActor<TAggregate>: PersistentActor where TAggregate : AggregateBase
    {
        protected TAggregate Aggregate;

        public AggregateActor(AggregateFactory factory)
        {
            var conventionName = AggregateActorName.Parse<TAggregate>(Self.Path.Name);

            PersistenceId = conventionName.ToString();
            Aggregate = factory.Build<TAggregate>(conventionName.Id);
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SnapshotOffer)
            {
                Aggregate = (TAggregate) (message as SnapshotOffer).Snapshot;
            }
            else ((IAggregate)Aggregate).ApplyEvent(message);
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            var commandType = message.GetType();
            Action<object> handler;
            if (!commandHandlers.TryGetValue(commandType, out handler))
                return false;

            handler.Invoke(message);
            return true;
        }

        private IDictionary<Type, Action<object>> commandHandlers = new Dictionary<Type, Action<object>>();
        protected void RegisterCommand<TCommand>(Action<TCommand> commandPass)
        {
            commandHandlers[typeof(TCommand)] = cmd => commandPass((TCommand)cmd);
        }

        public override string PersistenceId { get; } 
    }
}