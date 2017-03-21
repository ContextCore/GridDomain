using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public interface IFactory<out TResult, in TParam>
    {
        TResult Create(TParam message);
    }
}