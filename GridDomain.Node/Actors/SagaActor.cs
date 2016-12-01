using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Akka;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using Akka.Persistence;
using Automatonymous;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    //TODO: add status info, e.g. was any errors during execution or recover

    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TSagaState"></typeparam>
    public class SagaActor<TSaga, TSagaState> :
         EventSourcedActor<TSagaState> where TSaga : class,ISagaInstance 
        where TSagaState : AggregateBase
    {
        private readonly ISagaProducer<TSaga> _producer;
        private TSaga _saga;
        public TSaga Saga => _saga ?? (_saga = _producer.Create(State));
        private readonly HashSet<Type> _sagaStartMessageTypes;
        private readonly Dictionary<Type, string> _sagaIdFields;

        private Guid GetSagaId(DomainEvent msg)
        {
            var type = msg.GetType();
            string fieldName;
         
            if (_sagaIdFields.TryGetValue(type, out fieldName))
                return (Guid) type.GetProperty(fieldName).GetValue(msg);

            return msg.SagaId;
        }

        public SagaActor(ISagaProducer<TSaga> producer,
                         IPublisher publisher,
                         ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                         IConstructAggregates aggregatesConstructor)
                            :base(aggregatesConstructor,
                                  snapshotsPersistencePolicy,
                                  publisher)
        {
            _producer = producer;
            _sagaStartMessageTypes = new HashSet<Type>(producer.KnownDataTypes.Where(t => typeof(DomainEvent).IsAssignableFrom(t)));
            _sagaIdFields = producer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType, m => m.CorrelationField);

            //id from name is used due to saga.Data can be not initialized before messages not belonging to current saga will be received
            Command<DomainEvent>(msg =>
            {
                Monitor.IncrementMessagesReceived();
                if (_sagaStartMessageTypes.Contains(msg.GetType()))
                {
                    _saga = _producer.Create(msg);
                    State = _saga.Data;
                }

                ProcessSaga(msg);
            }, e => GetSagaId(e) == Id);

            Command<IFault>(fault =>
            {
                Monitor.IncrementMessagesReceived();
                ProcessSaga(fault);

            }, fault => fault.SagaId == Id);

        }
        private void ProcessSaga(object message)
        {
            try
            {
                Saga.Transit(message);
            }
            catch (Exception ex)
            {
                var processorType = _producer.Descriptor.StateMachineType;

                _log.Error(ex,"Saga {saga} {id} raised an error on {@message}", processorType, Id, message);
                var fault = Fault.NewGeneric(message, ex, processorType, Id);
                Publisher.Publish(fault);
            }

            var stateChange = ProcessSagaStateChange();

            ProcessSagaCommands();

            TrySaveSnapshot(stateChange);
        }

        private void ProcessSagaCommands()
        {
            foreach (var msg in Saga.CommandsToDispatch)
                 Publisher.Publish(msg);

            Saga.ClearCommandsToDispatch();
        }

        private object[] ProcessSagaStateChange()
        {
            var stateChangeEvents = State.GetUncommittedEvents().Cast<object>().ToArray();
            PersistAll(stateChangeEvents, 
                e => {
                         Publisher.Publish(e);
                         NotifyWatchers(new Persisted(e));
                });
            State.ClearUncommittedEvents();
            return stateChangeEvents;
        }

    }
}