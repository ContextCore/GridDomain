using System;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.Actors.Saga.Messages;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors.Saga
{
    public class SagaHubActor<TState> : PersistentHubActor where TState : class, ISagaState
    {
        private readonly ProcessEntry _redirectEntry;

        public SagaHubActor(IPersistentChildsRecycleConfiguration recycleConf): base(recycleConf, typeof(TState).Name)
        {
            _redirectEntry = new ProcessEntry(Self.Path.Name, "Forwarding to new child", "New saga was created");

            Receive<RedirectToNewSaga>(redirect =>
                                       {
                                           redirect.MessageToRedirect.Metadata.History.Add(_redirectEntry);
                                           var name = GetChildActorName(redirect.SagaId);
                                           SendToChild(redirect, redirect.SagaId, name);
                                       });
        }

        protected override string GetChildActorName(Guid childId)
        {
            return AggregateActorName.New<TState>(childId).ToString();
        }

        protected override Guid GetChildActorId(IMessageMetadataEnvelop env)
        {
            var childActorId = Guid.Empty;

            if (env.Message is RedirectToNewSaga saga)
                return saga.SagaId;

            env.Message.Match()
               .With<IFault>(m => childActorId = m.SagaId)
               .With<IHaveSagaId>(m => childActorId = m.SagaId);

            return childActorId;
        }

        protected override Type ChildActorType { get; } = typeof(SagaActor<TState>);

        protected override void SendMessageToChild(ChildInfo knownChild, object message)
        {
            var msgSender = Sender;
            var self = Self;
            knownChild.Ref
                      .Ask<ISagaTransitCompleted>(message)
                      .ContinueWith(t =>
                                    {
                                        t.Result.Match()
                                         .With<RedirectToNewSaga>(r => self.Tell(r, msgSender))
                                         .Default(r => msgSender.Tell(r, msgSender));
                                    });
        }
    }
}