﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using Akka.Persistence;
using Automatonymous;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    //TODO: add status info, e.g. was any errors during execution or recover

    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TSagaState"></typeparam>
    public class SagaActor<TSaga, TSagaState>: EventSourcedActor<TSagaState> where TSaga : class,ISagaInstance 
                                                                             where TSagaState : AggregateBase
    {
        private readonly ISagaProducer<TSaga> _producer;
        private TSaga _saga;
        public TSaga Saga => _saga ?? (_saga = _producer.Create(State));
        private readonly HashSet<Type> _sagaStartMessageTypes;
        private readonly Dictionary<Type, string> _sagaIdFields;
        private readonly ProcessEntry _sagaProducedEntry;
        private readonly ProcessEntry _exceptionOnTransit;

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
            Command<IMessageMetadataEnvelop<DomainEvent>>(m =>
            {
                var msg = m.Message;
                var metadata = m.Metadata;

                Monitor.IncrementMessagesReceived();
                if (_sagaStartMessageTypes.Contains(msg.GetType()))
                {
                    _saga = _producer.Create(msg);
                    State = _saga.Data;
                }

                //block any other executing until saga completes transition
                BecomeStacked(() => SagaProcessWaiting(msg));
                ProcessSaga(msg, metadata).PipeTo(Self,Sender);

            }, e => GetSagaId(e.Message) == Id);

            Command<IMessageMetadataEnvelop<IFault>>(m =>
            {
                var fault = m.Message;
                var metadata = m.Metadata;

                Monitor.IncrementMessagesReceived();

                //block any other executing until saga completes transition
                ProcessSaga(fault, metadata).PipeTo(Self,Sender);
                BecomeStacked(() => SagaProcessWaiting(fault));
            }, fault => fault.Message.SagaId == Id);

            _sagaProducedEntry = new ProcessEntry(Self.Path.Name, SagaActorLiterals.PublishingCommand, SagaActorLiterals.SagaProducedACommand);
            _exceptionOnTransit = new ProcessEntry(Self.Path.Name, SagaActorLiterals.CreatedFaultForSagaTransit, SagaActorLiterals.SagaTransitCasedAndError);
        }

        private void SagaProcessWaiting(object message)
        {
            CommandAny(o =>
            {
                o.Match()
                 .With<SagaTransited>(r =>
                 {
                     if (r.Error == null)
                     {
                         PersistState(r.Metadata);
                     }
                     else
                     {
                         PublishError(message, r.Metadata, r.Error.UnwrapSingle());
                     }

                     //notify saga process actor that saga transit is done
                     Sender.Tell(r);
                     Saga.ClearCommandsToDispatch();
                     Stash.UnstashAll();
                     UnbecomeStacked();
                 })
                 .Default(m => Stash.Stash());
            });
        }

        private void PublishError(object message, IMessageMetadata messageMetadata, Exception exception)
        {
            var processorType = _producer.Descriptor.StateMachineType;

            _log.Error(exception, "Saga {saga} {id} raised an error on {@message}", processorType, Id, message);
            var fault = Fault.NewGeneric(message, exception, Id, processorType);

            var metadata = messageMetadata.CreateChild(fault.SagaId, _exceptionOnTransit);

            Publisher.Publish(fault, metadata);
        }

        private Task<SagaTransited> ProcessSaga(object message, IMessageMetadata domainEventMetadata)
        {
            //cast is need for dynamic call of Transit
            Task processSagaTask = (Saga as ISagaInstance).Transit((dynamic) message);
            return processSagaTask.ContinueWith(t =>
            {
                if (t.IsFaulted)  return SagaTransited.CreateError(t.Exception);
                if (t.IsCanceled) return SagaTransited.CreateError(t.Exception as Exception ?? new TimeoutException());

                return new SagaTransited(Saga.CommandsToDispatch.ToArray(), domainEventMetadata);
            });
        }

        private void PersistState(IMessageMetadata mutatorMessageMetadata)
        {
            var stateChangeEvents = State.GetUncommittedEvents().Cast<DomainEvent>().ToArray();
            int totalEvents = stateChangeEvents.Length;
            int persistedEvents = 0;

            PersistAll(stateChangeEvents, 
            e =>
            {
                var metadata = mutatorMessageMetadata.CreateChild(e.SourceId, 
                                                                  new ProcessEntry(Self.Path.Name,
                                                                                   "Saga state event published",
                                                                                   "Saga changed state"));
                Publisher.Publish(e, metadata);
                NotifyPersistenceWatchers(new Persisted(e));
                //should save snapshot only after all messages persisted as state was already modified by all of them
                if(++persistedEvents == totalEvents)
                    TrySaveSnapshot(stateChangeEvents);

            });
            State.ClearUncommittedEvents();
        }

    }
}