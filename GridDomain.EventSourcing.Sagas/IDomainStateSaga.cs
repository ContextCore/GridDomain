using CommonDomain;

namespace GridDomain.EventSourcing.Sagas
{
    public interface IDomainStateSaga<T>: IDomainSaga where T: IAggregate
    {
        new T State { get; }
    }
}