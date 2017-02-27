using System;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging
{
    public class AggregateActorName
    {
        private static readonly string Separator = ":";

        internal AggregateActorName(Type aggregateType, Guid id)
        {
            Id = id;
            AggregateType = aggregateType;
            Name = aggregateType.BeautyName() + Separator + id;
        }

        public Type AggregateType { get; }
        public Guid Id { get; }

        public string Name { get; }

        public static AggregateActorName New<T>(Guid id)
        {
            return new AggregateActorName(typeof(T), id);
        }

        public static AggregateActorName Parse<T>(string value)
        {
            var aggregateType = typeof(T);
            var id = Guid.Parse(value.Replace(aggregateType.BeautyName() + Separator, ""));
            return new AggregateActorName(aggregateType, id);
        }

        public static bool TryParseId(string value, out Guid id)
        {
            var parts = value.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

            return Guid.TryParse(parts[1], out id);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}