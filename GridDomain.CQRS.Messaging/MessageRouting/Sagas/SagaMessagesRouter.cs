using System;
using CommonDomain.Persistence;
using GridDomain.EventSourcing;
using NLog;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    /// <summary>
    ///     Организует подписку саг на интересующие их события и публикацию результатирующих сообщений.
    ///     Постоянно висит в памяти и создаёт нужные саги при возникновении события
    /// </summary>
    public class SagaMessagesRouter<TSaga> where TSaga : IStatefullSaga
    {
        private readonly IMessagesRouter _messageRouter;
        private readonly IPublisher _publisher;
        private readonly IRepository _repo;
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();


        public SagaMessagesRouter(IRepository repo, IPublisher publisher, IMessagesRouter messagesRouter)
        {
            _repo = repo;
            _publisher = publisher;
            _messageRouter = messagesRouter;
        }

        public void RouteFailure<TCommand, TReason, TConcreteSaga>
            (Func<CommandFailure<TCommand, TReason>, TConcreteSaga> sagaFactory)
            where TCommand : ICommand
            where TConcreteSaga : TSaga, IMessageConsumer<CommandFailure<TCommand, TReason>>
        {
            Route(sagaFactory, fail => fail.Command.Id);
        }

        public void RouteCommand<TCommand, TConreteSaga>(Func<TCommand, TConreteSaga> sagaFactory)
            where TCommand : ICommand
            where TConreteSaga : TSaga, IMessageConsumer<TCommand>
        {
            Route(sagaFactory, cmd => cmd.Id);
        }

        public void RouteEvent<TCommand, TConreteSaga>(Func<TCommand, TConreteSaga> sagaFactory)
            where TCommand : DomainEvent
            where TConreteSaga : TSaga, IMessageConsumer<TCommand>
        {
            Route(sagaFactory, e => Guid.NewGuid());
        }

        public void Route<TMessage, TConreteSaga>(Func<TMessage, TConreteSaga> sagaFactory,
            Func<TMessage, Guid> commitIdFactory)
            where TConreteSaga : TSaga, IMessageConsumer<TMessage>
        {
            _messageRouter.Route<TMessage>()
                .To<SagaMessageHandler<TMessage, TConreteSaga>>()
                //.WithFactory(msg => 
                //new SagaMessageHandler<TMessage, TConreteSaga>(sagaFactory,
                //                                               commitIdFactory,
                //                                               _repo,
                //                                               _publisher))
                .Register();
        }
    }
}