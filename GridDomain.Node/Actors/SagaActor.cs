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

    class SagaFactoryAdapter<TSaga, TMessage> :  ISagaFactory<TSaga, object> where TSaga : ISagaInstance
    {
        private readonly ISagaFactory<TSaga, TMessage> _factory;

        public TSaga Create(object message)
        {
            return _factory.Create((TMessage) message);
        }

        public SagaFactoryAdapter(ISagaFactory<TSaga, TMessage> factory)
        {
            _factory = factory;
        }
    }

    public class SagaActor<TSaga, TSagaState, TStartMessage> : SagaActor<TSaga, TSagaState>
        where TSaga : class, ISagaInstance
        where TSagaState : AggregateBase
        where TStartMessage : DomainEvent
    {
        public SagaActor(ISagaFactory<TSaga, TStartMessage> sagaStarter,
                         ISagaFactory<TSaga, TSagaState> sagaFactory, 
                         AggregateFactory aggregateFactory,
                         IPublisher publisher):
                         base(new SagaFactoryAdapter<TSaga,TStartMessage>(sagaStarter), sagaFactory, aggregateFactory, publisher,new [] {typeof(TStartMessage)})
        {
        }
    }

        /// <summary>
        ///     Name should be parse by AggregateActorName
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <typeparam name="TSagaState"></typeparam>
        /// <typeparam name="TStartMessage"></typeparam>
        public class SagaActor<TSaga, TSagaState> :
        ReceivePersistentActor where TSaga : class,ISagaInstance 
        where TSagaState : AggregateBase
    {
        private readonly IPublisher _publisher;
        private readonly ISagaFactory<TSaga, object> _sagaStarter;
        private readonly ISagaFactory<TSaga, TSagaState> _sagaFactory;


        private TSaga _saga;
        private TSagaState _sagaData;
        public readonly Guid Id;
        public TSaga Saga => _saga ?? (_saga = _sagaFactory.Create(_sagaData));


        public SagaActor(ISagaFactory<TSaga, object> sagaStarter,
                         ISagaFactory<TSaga, TSagaState> sagaFactory,
                         AggregateFactory aggregateFactory,
                         IPublisher publisher,
                         Type[] startMessages)
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
                var type = msg.GetType();
                if(startMessages.Any(t => t.IsAssignableFrom(type)))
                        _saga = _sagaStarter.Create(msg);

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