using System;
using System.Linq;
using CommonDomain.Persistence;
using GridDomain.EventSourcing;
using NLog;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    internal class SagaMessageHandler<TMessage, TSaga> : IHandler<TMessage>
        where TSaga : IMessageConsumer<TMessage>, IStatefullSaga

    {
        private readonly IPublisher _bus;
        private readonly Func<TMessage, Guid> _commitIdFactory;
        private readonly Func<TMessage, TSaga> _factory;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IRepository _repo;

        public SagaMessageHandler(Func<TMessage, TSaga> factory,
            Func<TMessage, Guid> commitIdFactory,
            IRepository repo,
            IPublisher bus)
        {
            _commitIdFactory = commitIdFactory;
            _repo = repo;
            _bus = bus;
            _factory = factory;
        }

        public void Handle(TMessage e)
        {
            var saga = _factory.Invoke(e);
            var messageIsAcceptable = false;

            try
            {
                messageIsAcceptable = saga.IsAcceptable(e);
            }
            catch (Exception ex)
            {
                _log.Fatal($"Сага {saga.GetType().Name} при проверке применимости к ней сообщения" +
                           $" {typeof (TMessage).Name} породила ошибку:\r\n{ex}");
                return;
            }

            if (messageIsAcceptable) saga.Handle(e);
            else
            {
                var consumerName = saga.GetType().Name;
                _log.Trace(
                    $"Сообщение {typeof (TMessage).Name}, предназначавшееся для {consumerName} не было обработано из-за правила фильтрации");
            }

            _repo.Save(saga.State, _commitIdFactory(e));

            var msgs = saga.State.GetUncommittedEvents().Cast<object>().ToArray();
            saga.ClearUndispatchedMessages();

            foreach (var cmd in msgs)
            {
                _log.Trace($"сага {typeof (TSaga).Name} публикует сообщение {cmd.GetType().Name}");
                _bus.Publish(cmd);
            }
        }
    }
}