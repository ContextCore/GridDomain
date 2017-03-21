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
    public class SagaHubActor<TMachine, TState> : PersistentHubActor where TMachine : SagaStateMachine<TState>
                                                                      where TState : class,ISagaState
    {
        private readonly Dictionary<Type, string> _acceptMessagesSagaIds;
        private readonly Type _actorType = typeof(SagaActor<TState>);

        public SagaHubActor(IPersistentChildsRecycleConfiguration recycleConf, ISagaProducer<TState> sagaProducer)
            : base(recycleConf, typeof(TMachine).Name)
        {
            _acceptMessagesSagaIds = sagaProducer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType,
                                                                                         m => m.CorrelationField);
        }

        protected override string GetChildActorName(object message)
        {
            return AggregateActorName.New<TState>(GetChildActorId(message)).ToString();
        }

        protected override Guid GetChildActorId(object message)
        {
            var childActorId = Guid.Empty;

            message.Match().With<IFault>(m => childActorId = m.SagaId);

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

        protected override Type GetChildActorType(object message)
        {
            return _actorType;
        }

        protected override void SendMessageToChild(ChildInfo knownChild, IMessageMetadataEnvelop message)
        {
            knownChild.Ref.Ask<ISagaTransitCompleted>(message).PipeTo(Sender, Self);
        }
    }
}