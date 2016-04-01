using System;
using CommonDomain;
using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;

namespace GridDomain.CQRS.Messaging.MessageRouting.CommandHandlerBuilders
{
    public class AggregateActionBuilder<TMessage, TAggregate> where TAggregate : IAggregate where TMessage : ICommand
    {
        private readonly Func<TMessage, TAggregate> _aggregateFactory;
        private readonly Func<TMessage, Guid> _commitIdFactory;
        private readonly IPublisher _publisher;
        private readonly IRepository _repository;
        private readonly Func<TMessage, Guid> _sagaIdFactory;

        public AggregateActionBuilder(Func<TMessage, TAggregate> aggregateFactory,
            Func<TMessage, Guid> sagaIdFactory,
            Func<TMessage, Guid> commitIdFactory,
            IRepository repository,
            IPublisher publisher)
        {
            _publisher = publisher;
            _commitIdFactory = commitIdFactory;
            _repository = repository;
            _sagaIdFactory = sagaIdFactory;
            _aggregateFactory = aggregateFactory;
        }

        public FailureBuilder<TMessage, TAggregate> Action(Action<TMessage, TAggregate> act)
        {
            return new FailureBuilder<TMessage, TAggregate>(
                msg => ProcessAggregateCommand(act, msg),
                _publisher);
        }

        public FailureBuilder<TMessage, TAggregate> NoAction()
        {
            return Action((t, a) => { });
        }

        private TAggregate ProcessAggregateCommand(Action<TMessage, TAggregate> act, TMessage msg)
        {
            var aggregate = _aggregateFactory(msg);
            act(msg, aggregate);

            var sagaId = _sagaIdFactory(msg);
            SagaEventCorrelator.MarkEventsBelongingToSaga(aggregate, sagaId);
            _repository.Save(aggregate, _commitIdFactory(msg));

            return aggregate;
        }
    }
}