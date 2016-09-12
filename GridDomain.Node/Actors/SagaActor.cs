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
    class CheckHealth
    {
        public string Payload {get;}

        public CheckHealth(string payload = null)
        {
            this.Payload = payload;
        }
    }

    //TODO: add status info, e.g. was any errors during execution or recover
    class HealthStatus
    {
        public string Payload { get; }

        public HealthStatus(string payload = null)
        {
            this.Payload = payload;
        }
    }

    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TSagaState"></typeparam>
    public class SagaActor<TSaga, TSagaState> :
        ReceivePersistentActor where TSaga : class,ISagaInstance 
        where TSagaState : AggregateBase
    {
        private readonly ISagaProducer<TSaga> _producer;
        private readonly IPublisher _publisher;
        private TSaga _saga;
        private IAggregate _sagaData;
        public readonly Guid Id;
        public TSaga Saga => _saga ?? (_saga = _producer.Create(_sagaData));
        private readonly AggregateFactory _aggregateFactory = new AggregateFactory();
        private readonly HashSet<Type> _sagaStartMessageTypes;

        private Guid GetSagaId(DomainEvent msg)
        {
            var type = msg.GetType();
            string fieldName;
         
            if (_sagaIdFIelds.TryGetValue(type, out fieldName))
                return (Guid) type.GetProperty(fieldName).GetValue(msg);

            return msg.SagaId;
        }

        public SagaActor(ISagaProducer<TSaga> producer,
                         IPublisher publisher)
        {
            _producer = producer;
            _publisher = publisher;
            _sagaStartMessageTypes = new HashSet<Type>(producer.KnownDataTypes.Where(t => typeof(DomainEvent).IsAssignableFrom(t)));
            _sagaIdFIelds = producer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType, m => m.CorrelationField);

            //id from name is used due to saga.Data can be not initialized before messages not belonging to current saga will be received
            Id = AggregateActorName.Parse<TSagaState>(PersistenceId).Id;
            _sagaData = _aggregateFactory.Build<TSagaState>(Id);


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

            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Command<DomainEvent>(msg =>
            {
                _monitor.IncrementMessagesReceived();
                if (_sagaStartMessageTypes.Contains(msg.GetType()))
                    _saga = _producer.Create(msg);

                ProcessSaga(msg);
            }, e => GetSagaId(e) == Id);

            //recover messages will be provided only to right saga by using peristenceId
            Recover<SnapshotOffer>(offer => _sagaData = (IAggregate)offer.Snapshot);
            Recover<DomainEvent>(e => _sagaData.ApplyEvent(e));
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
            foreach (var msg in Saga.CommandsToDispatch)
                      _publisher.Publish(msg);

            Saga.ClearCommandsToDispatch();
        }

        private void ProcessSagaStateChange()
        {
            var stateChangeEvents = Saga.Data.GetUncommittedEvents().Cast<object>();

            PersistAll(stateChangeEvents, e => _publisher.Publish(e));

            Saga.Data.ClearUncommittedEvents();
        }

        private readonly ActorMonitor _monitor = new ActorMonitor(Context, typeof(TSaga).Name);
        private Dictionary<Type, string> _sagaIdFIelds;

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