using System;
using CommonDomain;
using CommonDomain.Persistence;

namespace GridDomain.CQRS.Messaging.MessageRouting.CommandHandlerBuilders
{
    public class AggregateCreationBuilder<TCommand, TAggregate> where TCommand : ICommand
        where TAggregate : class, IAggregate
    {
        private readonly IPublisher _publisher;
        private readonly IRepository _repository;

        public AggregateCreationBuilder(IRepository repository, IPublisher publisher)
        {
            _publisher = publisher;
            _repository = repository;
        }

        public AggregateActionBuilder<TCommand, TAggregate> LoadedById(Func<TCommand, Guid> aggregateIdFactory)
        {
            return new AggregateActionBuilder<TCommand, TAggregate>
                (msg => _repository.GetById<TAggregate>(aggregateIdFactory(msg)),
                    cmd => cmd.SagaId,
                    cmd => cmd.Id,
                    _repository,
                    _publisher);
        }

        public AggregateActionBuilder<TCommand, TAggregate> CreatedBy(Func<TCommand, TAggregate> aggregateFactory)
        {
            return new AggregateActionBuilder<TCommand, TAggregate>
                (aggregateFactory,
                    cmd => cmd.SagaId,
                    cmd => cmd.Id,
                    _repository,
                    _publisher);
        }
    }
}