using CommonDomain;

namespace GridDomain.EventSourcing.Sagas
{
    public interface IDomainStateSaga<T>: ISagaInstance where T: IAggregate
    {
        new T State { get; }
    }
}