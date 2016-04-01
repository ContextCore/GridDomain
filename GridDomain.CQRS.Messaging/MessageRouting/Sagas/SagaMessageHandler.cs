using System;
using System.Linq;
using CommonDomain.Persistence;
using GridDomain.EventSourcing;
using NLog;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    class SagaMessageHandler<TMessage, TSaga> : IHandler<TMessage> where TSaga : IMessageConsumer<TMessage>, IStatefullSaga

    {
        private readonly Func<TMessage, TSaga> _factory;
        private readonly IPublisher _bus;
        private readonly IRepository _repo;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Func<TMessage, Guid> _commitIdFactory;

        public SagaMessageHandler(Func<TMessage, TSaga> factory,
                                           Func<TMessage,Guid> commitIdFactory,
                                           IRepository repo, 
                                           IPublisher bus)
        {
            _commitIdFactory = commitIdFactory;
            _repo = repo;
            _bus = bus;
            _factory = factory;
        }

        public void Handle(TMessage msg)
        {
            var saga = _factory.Invoke(msg);
            bool messageIsAcceptable = false;

            try
            {
                messageIsAcceptable = saga.IsAcceptable(msg);
            }
            catch (Exception ex)
            {
                _log.Fatal($"Сага {saga.GetType().Name} при проверке применимости к ней сообщения" +
                           $" {typeof(TMessage).Name} породила ошибку:\r\n{ex}");
                return;
            }

            if (messageIsAcceptable) saga.Handle(msg);
            else
            {
                var consumerName = saga.GetType().Name;
                _log.Trace($"Сообщение {typeof(TMessage).Name}, предназначавшееся для {consumerName} не было обработано из-за правила фильтрации");
            }

            _repo.Save(saga.State, _commitIdFactory(msg));
 
            var msgs = saga.GetUndispatchedMessages().Cast<object>().ToArray();
            saga.ClearUndispatchedMessages();

            foreach (var cmd in msgs)
            {
                _log.Trace($"сага {typeof(TSaga).Name} публикует сообщение {cmd.GetType().Name}");
                _bus.Publish(cmd);
            }
        }
    }
}