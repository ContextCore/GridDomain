using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;

namespace GridDomain.Node
{
    public static class DomainEventAdapters
    {
        public static void Register(JsonConverter converter)
        {
            DomainEventsJsonSerializer.Register(converter);
        }

        public static void Clear()
        {
            DomainEventsJsonSerializer.Clear();
        }

        public static void Register<TFrom, TTo>(ObjectAdapter<TFrom, TTo> converter)
        {
            DomainEventsJsonSerializer.Register(converter);
        }
    }
}