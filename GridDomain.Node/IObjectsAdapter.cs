using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;

namespace GridDomain.Node
{
    public interface IObjectsAdapter
    {
        void Register(JsonConverter converter);
        //{
        //    DomainEventsJsonSerializer.Register(converter);
        //}

        void Clear();

        void Register<TFrom, TTo>(ObjectAdapter<TFrom, TTo> converter);
        //{
        //    DomainEventsJsonSerializer.Register(converter);
        //}
    }
}