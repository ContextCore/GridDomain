using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class SagaHubActor<TSaga, TSagaState> : PersistentHubActor where TSaga : class, ISagaInstance
                                                                      where TSagaState : AggregateBase
    {
        private readonly Dictionary<Type, string> _acceptMessagesSagaIds;
        private readonly Type _actorType = typeof(SagaActor<TSaga, TSagaState>);

        public SagaHubActor(IPersistentChildsRecycleConfiguration recycleConf, ISagaProducer<TSaga> sagaProducer)
            : base(recycleConf, typeof(TSaga).Name)
        {
            _acceptMessagesSagaIds = sagaProducer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType,
                m => m.CorrelationField);
        }

        protected override string GetChildActorName(object message)
        {
            return AggregateActorName.New<TSagaState>(GetChildActorId(message))
                                     .ToString();
        }

        protected override Guid GetChildActorId(object message)
        {
            var childActorId = Guid.Empty;

            message.Match()
                   .With<IFault>(m => childActorId = m.SagaId);

            if (childActorId != Guid.Empty) return childActorId;

            string fieldName;
            var type = message.GetType();

            if (_acceptMessagesSagaIds.TryGetValue(type, out fieldName))
                childActorId = (Guid) type.GetProperty(fieldName)
                                          .GetValue(message);
            else
            {
                //try to search by inheritance
                var firstInherited = _acceptMessagesSagaIds.FirstOrDefault(i => i.Key.IsAssignableFrom(type));
                var sagaIdField = firstInherited.Value;

                childActorId = (Guid) type.GetProperty(sagaIdField)
                                          .GetValue(message);
            }
            return childActorId;
        }

        protected override Type GetChildActorType(object message)
        {
            return _actorType;
        }

        protected override void SendMessageToChild(ChildInfo knownChild, IMessageMetadataEnvelop message)
        {
            knownChild.Ref.Ask<ISagaTransitCompleted>(message)
                      .PipeTo(Sender, Self);
        }
    }
}