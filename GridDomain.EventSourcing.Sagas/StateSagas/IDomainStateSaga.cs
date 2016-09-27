using System;
using CommonDomain;

namespace GridDomain.EventSourcing.Sagas.StateSagas
{
   // [Obsolete("Use Saga classes instead")]
    public interface IDomainStateSaga<T>: ISagaInstance where T: IAggregate
    {
        T State { get; }
    }
}