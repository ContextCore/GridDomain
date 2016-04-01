using CommonDomain;

namespace GridDomain.EventSourcing
{
    public interface IStatefullSaga : ISaga
    {
        IAggregate State { get; }
    }
}