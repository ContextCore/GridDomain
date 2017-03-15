﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.CommandPipe;
using ISaga = GridDomain.EventSourcing.Sagas.ISaga;

namespace GridDomain.Node.Actors
{
    //TODO: add status info, e.g. was any errors during execution or recover

    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TMachine"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public class SagaActor<TMachine, TState> : AggregateActor<SagaStateAggregate<TState>> where TMachine : SagaStateMachine<TState>
                                                                                          where TState : class, ISagaState
    {
        private readonly ProcessEntry _exceptionOnTransit;
        private readonly ISagaProducer<ISaga<TMachine, TState>> _producer;
        private readonly IPublisher _publisher;
        private readonly Dictionary<Type, string> _sagaIdFields;
        private readonly ProcessEntry _sagaProducedCommand;
        private readonly HashSet<Type> _sagaStartMessageTypes;
        private readonly ProcessEntry _stateChanged;

        private ISaga<TMachine,TState> _saga;

        public SagaActor(ISagaProducer<ISaga<TMachine, TState>> producer,
                         IPublisher publisher,
                         IActorRef schedulerActorRef,
                         IActorRef customHandlersActorRef,
                         ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                         IConstructAggregates aggregatesConstructor)

            : base(new SagaStateCommandHandler<TState>(), schedulerActorRef,
                   publisher,
                   snapshotsPersistencePolicy,
                   aggregatesConstructor,
                   customHandlersActorRef)
        {
            _publisher = publisher;
            _producer = producer;
            _sagaStartMessageTypes = new HashSet<Type>(producer.KnownDataTypes.Where(t => typeof(DomainEvent).IsAssignableFrom(t)));
            _sagaIdFields = producer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType, m => m.CorrelationField);

            _exceptionOnTransit = new ProcessEntry(Self.Path.Name,
                                                   SagaActorLiterals.CreatedFaultForSagaTransit,
                                                   SagaActorLiterals.SagaTransitCasedAndError);
            _stateChanged = new ProcessEntry(Self.Path.Name, "Saga state event published", "Saga changed state");
            _sagaProducedCommand = new ProcessEntry(Self.Path.Name,
                                                    SagaActorLiterals.PublishingCommand,
                                                    SagaActorLiterals.SagaProducedACommand);

            //id from name is used due to saga.Data can be not initialized before messages not belonging to current saga will be received
            Command<IMessageMetadataEnvelop<DomainEvent>>(CreateSagaIfNeed,
                                                          e => GetSagaId(e.Message) == Id);

            Command<IMessageMetadataEnvelop<IFault>>(CreateSagaIfNeed,
                                                     e => GetSagaId(e.Message) == Id);
        }

        private void CreateSagaIfNeed(IMessageMetadataEnvelop m)
        {
            var msg = m.Message;
            var metadata = m.Metadata;

            Monitor.IncrementMessagesReceived();
            if (_sagaStartMessageTypes.Contains(msg.GetType()))
            {
                _saga = _producer.Create(msg);

                var cmd = new CreateNewStateCommand<TState>(Id, _saga.State);

                Self.Ask<CommandExecuted>(cmd)
                    .PipeTo(Self);

                BecomeStacked(() => SagaInitializing(msg, metadata));
                return;
            }

            TransitSaga(msg, metadata);
        }

        private void TransitSaga(object msg, IMessageMetadata metadata)
        {
            //block any other executing until saga completes transition
            //cast is need for dynamic call of Transit
            Task processSagaTask = (Saga as ISaga).Transit((dynamic) msg);
            processSagaTask.ContinueWith(t => new SagaTransited(Saga.CommandsToDispatch.ToArray(),
                                                                metadata,
                                                                _sagaProducedCommand,
                                                                null))
                           .PipeTo(Self, Sender);

            BecomeStacked(() => SagaProcessWaiting(msg, metadata));
        }

        private void SagaInitializing(object msg, IMessageMetadata metadata)
        {
            CommandAny(c =>
                       {
                           c.Match()
                            .With<CommandExecuted>(cmd =>
                                                   {
                                                       UnbecomeStacked();
                                                       TransitSaga(msg, metadata);
                                                   })
                            .Default(mbox => Stash.Stash());
                       });
        }

        public ISaga<TMachine,TState> Saga => _saga ?? (_saga = _producer.Create(State));

        private Guid GetSagaId(object msg)
        {
            var type = msg.GetType();
            string fieldName;

            if (_sagaIdFields.TryGetValue(type, out fieldName))
                return (Guid) type.GetProperty(fieldName).GetValue(msg);

            throw new CannotFindSagaIdException(msg);
        }

        protected override void Terminating()
        {
            Command<IMessageMetadataEnvelop<DomainEvent>>(m =>
                                                          {
                                                              Self.Tell(CancelShutdownRequest.Instance);
                                                              Stash.Stash();
                                                          });
            Command<IMessageMetadataEnvelop<IFault>>(m =>
                                                     {
                                                         Self.Tell(CancelShutdownRequest.Instance);
                                                         Stash.Stash();
                                                     });
            base.Terminating();
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Usially it is domain event or fault</param>
        /// <param name="messageMetadata"></param>
        private void SagaProcessWaiting(object message, IMessageMetadata messageMetadata)
        {
            CommandAny(o =>
                       {
                           o.Match()
                            .With<SagaTransited>(r =>
                                                 {
                                                     var stateChangeCommand = new SaveStateCommand<TState>(Id, (TState) r.NewSagaState, "", message.GetType());
                                                     Self.Ask<CommandExecuted>(stateChangeCommand)
                                                         .ContinueWith(t => NotifySenderAndResume(r));
                                                     // PersistState(messageMetadata);
                                                 })
                            .With<Status.Failure>(f =>
                                                  {
                                                      Saga.ClearCommandsToDispatch();
                                                      var fault = PublishError(message,
                                                                               messageMetadata,
                                                                               f.Cause.UnwrapSingle());

                                                      NotifySenderAndResume(new SagaTransitFault(fault, messageMetadata));
                                                  })
                            .Default(m => Stash.Stash());
                       });
        }

        private void NotifySenderAndResume(object message)
        {
            //notify saga process actor that saga transit is done
            Sender.Tell(message);
            Stash.UnstashAll();
            UnbecomeStacked();
        }

        protected override void FinishCommandExecution(ICommand cmd, IMessageMetadata metadata, IActorRef commandSender)
        {
            base.FinishCommandExecution(cmd, metadata, commandSender);
            commandSender.Tell(new CommandExecuted(cmd.Id));
        }

        private IFault PublishError(object message, IMessageMetadata messageMetadata, Exception exception)
        {
            var processorType = _producer.Descriptor.StateMachineType;

            Log.Error(exception, "Saga {saga} {id} raised an error on {@message}", processorType, Id, message);
            var fault = Fault.NewGeneric(message, exception, Id, processorType);

            var metadata = messageMetadata.CreateChild(fault.SagaId, _exceptionOnTransit);

            _publisher.Publish(fault, metadata);
            return fault;
        }

        private void PersistState(IMessageMetadata mutatorMessageMetadata)
        {
            var stateChangeEvents = ((IAggregate) State).GetUncommittedEvents().Cast<DomainEvent>().ToArray();
            var totalEvents = stateChangeEvents.Length;
            var persistedEvents = 0;

            PersistAll(stateChangeEvents,
                       e =>
                       {
                           NotifyPersistenceWatchers(new Persisted(e));
                           //should save snapshot only after all messages persisted as state was already modified by all of them
                           if (++persistedEvents == totalEvents)
                           {
                               //   SnapshotsPolicy.MarkEventsProduced(stateChangeEvents.Length);
                               foreach (var e1 in stateChangeEvents)
                               {
                                   var metadata = mutatorMessageMetadata.CreateChild(e1.SourceId, _stateChanged);
                                   _publisher.Publish(e1, metadata);
                               }
                               TrySaveSnapshot();
                           }
                       });

            ((IAggregate) State).ClearUncommittedEvents();
        }
    }

    public class CommandExecuted
    {
        public Guid CommandId { get; }

        public CommandExecuted(Guid commandId)
        {
            CommandId = commandId;
        }

        public static CommandExecuted Instance { get; } = new CommandExecuted(Guid.Empty);
    }

    internal class CannotFindSagaIdException : Exception
    {
        public object Msg { get; }

        public CannotFindSagaIdException(object msg)
        {
            Msg = msg;
        }
    }
}