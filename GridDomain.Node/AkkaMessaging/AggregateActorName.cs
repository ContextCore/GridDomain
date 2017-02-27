using System;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging
{
    public class AggregateActorName
    {
        private static readonly string Separator = "_";

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
        public static bool TryParse<T>(string value, out AggregateActorName name)
        {
            var aggregateType = typeof(T);
            Guid id;
            if (!Guid.TryParse(value.Replace(aggregateType.BeautyName() + Separator, ""), out id))
            {
                name = null;
                return false;
            }
            name = new AggregateActorName(aggregateType, id);
            return true;
        }

        public static AggregateActorName ParseDynamic(string value)
        {
            var parts = value.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new BadNameFormatException();

            var type = Type.GetType(parts[0]);
            if (type == null)
                throw new CannotFindIdTypeException();

            Guid id;
            if (!Guid.TryParse(parts[1], out id))
                throw new IdParseException();

            return new AggregateActorName(type, id);
        }

        public static bool TryParse(string value, out AggregateActorName name)
        {
            try
            {
                name = ParseDynamic(value);
                return true;
            }
            catch
            {
                name = null;
                return false;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}