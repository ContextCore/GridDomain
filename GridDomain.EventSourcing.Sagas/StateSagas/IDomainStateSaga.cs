using System;
using CommonDomain;

namespace GridDomain.EventSourcing.Sagas.StateSagas
{
   // [Obsolete("Use Saga classes instead")]
    public interface IDomainStateSaga<T>: ISagaInstance where T: IAggregate
    {
        new T State { get; }
    }
}