using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class SagaHubActor<TMachine, TState> : PersistentHubActor where TMachine : Process<TState>
                                                                     where TState : class, ISagaState
    {
        private readonly Dictionary<Type, string> _acceptMessagesSagaIds;
        private readonly Type _actorType = typeof(SagaActor<TState>);

        public SagaHubActor(IPersistentChildsRecycleConfiguration recycleConf, ISagaProducer<TState> sagaProducer)
            : base(recycleConf, typeof(TMachine).Name)
        {
            _acceptMessagesSagaIds = sagaProducer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType,
                                                                                         m => m.CorrelationField);

            Receive<IMessageMetadataEnvelop>(messageWithMetadata =>
                                             {
                                                 messageWithMetadata.Match()
                                                                    .With<RedirectToNewSaga>(r =>
                                                                                             {
                                                                                                 var name = GetChildActorName(messageWithMetadata, r.SagaId);
                                                                                                 SendToChild(messageWithMetadata, r.SagaId, name);
                                                                                             })
                                                                    .Default(r =>
                                                                             {
                                                                                 var childId = GetChildActorId(messageWithMetadata);
                                                                                 var name = GetChildActorName(messageWithMetadata, childId);
                                                                                 SendToChild(messageWithMetadata, childId, name);
                                                                             });
                                             });
        }

        protected override string GetChildActorName(IMessageMetadataEnvelop message, Guid childId)
        {
            return AggregateActorName.New<TState>(childId).ToString();
        }

        protected override Guid GetChildActorId(IMessageMetadataEnvelop env)
        {
            var message = env.Message;
            var childActorId = Guid.Empty;

            message.Match().With<IFault>(m => childActorId = m.SagaId);
            env.Match().With<RedirectToNewSaga>(r => childActorId = r.SagaId);

            if (childActorId != Guid.Empty)
                return childActorId;

            string fieldName;
            var type = message.GetType();

            if (_acceptMessagesSagaIds.TryGetValue(type, out fieldName))
            {
                childActorId = (Guid) type.GetProperty(fieldName).GetValue(message);
            }
            else
            {
                //try to search by inheritance
                var firstInherited = _acceptMessagesSagaIds.FirstOrDefault(i => i.Key.IsAssignableFrom(type));
                var sagaIdField = firstInherited.Value;

                childActorId = (Guid) type.GetProperty(sagaIdField).GetValue(message);
            }
            return childActorId;
        }

        protected override Type GetChildActorType(IMessageMetadataEnvelop message)
        {
            return _actorType;
        }

        protected override void SendMessageToChild(ChildInfo knownChild, IMessageMetadataEnvelop message)
        {
            var msgSender = Sender;
            var self = Self;
            //  message.Match().With<RedirectToNewSaga>(r => SendMessageToChild(knownChild, r.))
            knownChild.Ref
                      .Ask<ISagaTransitCompleted>(message)
                      .ContinueWith(t =>
                                    {
                                        t.Result.Match()
                                         .With<RedirectToNewSaga>(r => self.Tell(message))
                                         .Default(r => msgSender.Tell(r));
                                    });
        }
    }
}