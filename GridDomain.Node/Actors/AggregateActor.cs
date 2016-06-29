using System;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : ReceivePersistentActor where TAggregate : AggregateBase
    {
        private readonly IAggregateCommandsHandler<TAggregate> _handler;
        private readonly IPublisher _publisher;

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              AggregateFactory factory,
                              IPublisher publisher)
        {
            _handler = handler;
            _publisher = publisher;
            PersistenceId = Self.Path.Name;
            Aggregate = factory.Build<TAggregate>(AggregateActorName.Parse<TAggregate>(Self.Path.Name).Id);

            Command<ICommand>(cmd =>
            {
                //TODO: create more efficient way to set up a saga
                try
                {
                    Aggregate = _handler.Execute(Aggregate, cmd);
                }
                catch (Exception ex)
                {
                    _publisher.Publish(CommandFaultFactory.CreateGenericFor(cmd,ex));
                    return;
                }
                
                var aggregate = (IAggregate) Aggregate;
                
                var events = aggregate.GetUncommittedEvents()
                    .Cast<DomainEvent>()
                    .Select(e => e.CloneWithSaga(cmd.SagaId));

                PersistAll(events, e => _publisher.Publish(e));
                aggregate.ClearUncommittedEvents();
            });

            Recover<SnapshotOffer>(offer => Aggregate = (TAggregate) offer.Snapshot);
            Recover<DomainEvent>(e => ((IAggregate) Aggregate).ApplyEvent(e));
        }

        public TAggregate Aggregate { get; private set; }
        public override string PersistenceId { get; }

        public override void AroundPreRestart(Exception cause, object message)
        {
            base.AroundPreRestart(cause, message);
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            return base.AroundReceive(receive, message);
        }

    }
}