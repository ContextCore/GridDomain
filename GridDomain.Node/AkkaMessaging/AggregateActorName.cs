using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class AggregateActorName
    {
        public Type AggregateType { get; }
        public Guid Id { get; }
        private static string Separator = "_";


        private AggregateActorName(Type aggregateType, Guid id)
        {
            Id = id;
            AggregateType = aggregateType;
            Name = aggregateType.Name + Separator + id;
        }

        public string Name { get; }

        public static AggregateActorName New<T>(Guid id)
        {
            return new AggregateActorName(typeof (T), id);
        }

        public static AggregateActorName Parse<T>(string value)
        {
            var aggregateType = typeof(T);
            var id = Guid.Parse(value.Replace(aggregateType.Name + Separator, ""));
            return new AggregateActorName(aggregateType, id);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}