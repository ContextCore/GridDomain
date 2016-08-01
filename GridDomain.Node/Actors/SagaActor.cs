using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using Akka.Persistence;
using Automatonymous;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TSagaState"></typeparam>
    /// <typeparam name="TStartMessage"></typeparam>
    public class SagaActor<TSaga, TSagaState, TStartMessage> :
        ReceivePersistentActor where TSaga : class,ISagaInstance 
        where TSagaState : AggregateBase
        where TStartMessage : DomainEvent
    {
        private readonly IPublisher _publisher;
        private readonly ISagaFactory<TSaga, TStartMessage> _sagaStarter;
        private readonly ISagaFactory<TSaga, TSagaState> _sagaFactory;


        private TSaga _saga;
        private TSagaState _sagaData;
        public readonly Guid Id;
        public TSaga Saga => _saga ?? (_saga = _sagaFactory.Create(_sagaData));


        public SagaActor(ISagaFactory<TSaga, TStartMessage> sagaStarter,
                         ISagaFactory<TSaga, TSagaState> sagaFactory,
                         AggregateFactory aggregateFactory,
                         IPublisher publisher)
        {
            _sagaStarter = sagaStarter;
            _sagaFactory = sagaFactory;
            _publisher = publisher;
            _monitor = new ActorMonitor(Context,typeof(TSaga).Name);

            //id from name is used due to saga.Data can be not initialized before messages not belonging to current saga will be received
            Id = AggregateActorName.Parse<TSagaState>(PersistenceId).Id;
            _sagaData = aggregateFactory.Build<TSagaState>(Id);

            Command<ICommandFault>(fault =>
            {
                _monitor.IncrementMessagesReceived();
                ProcessSaga(fault);
            }, fault => fault.SagaId == Id);

            Command<ShutdownRequest>(req =>
            {
                _monitor.IncrementMessagesReceived();
                Shutdown();
            });

            Command<DomainEvent>(msg =>
            {
                _monitor.IncrementMessagesReceived();
               msg.Match().With<TStartMessage>(start => _saga = _sagaStarter.Create(start));
               ProcessSaga(msg);
            }, e => e.SagaId == Id);

            //recover messages will be provided only to right saga by using peristenceId
            Recover<SnapshotOffer>(offer => _sagaData = (TSagaState)offer.Snapshot);
            Recover<DomainEvent>(e => ((IAggregate)_sagaData).ApplyEvent(e));
        }


        protected virtual void Shutdown()
        {
            Context.Stop(Self);
        }

        private void ProcessSaga(object message)
        {
            Saga.Transit(message);

            ProcessSagaStateChange();

            ProcessSagaCommands();
        }

        private void ProcessSagaCommands()
        {
            foreach (var msg in Saga.CommandsToDispatch
                .OfType<Command>()
                .Select(c => c.CloneWithSaga(Saga.Data.Id)))
                _publisher.Publish(msg);

            Saga.ClearCommandsToDispatch();
        }

        private void ProcessSagaStateChange()
        {
            var stateChangeEvents = Saga.Data.GetUncommittedEvents().Cast<object>();

            PersistAll(stateChangeEvents, e => { _publisher.Publish(e); });

            Saga.Data.ClearUncommittedEvents();
        }

        private readonly ActorMonitor _monitor;

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
        }
        protected override void PreRestart(Exception reason, object message)
        {
            _monitor.IncrementActorRestarted();
        }

        public override string PersistenceId => Self.Path.Name;
    }
}