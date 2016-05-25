using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.EventSourcing
{
    public interface IStatefullSaga : ISaga
    {
        IAggregate State { get; }
    }

    //public interface ICommandHandler<in TCommand>
    //{
    //    IReadOnlyCollection<DomainEvent> Execute(TCommand command);
    //}
}