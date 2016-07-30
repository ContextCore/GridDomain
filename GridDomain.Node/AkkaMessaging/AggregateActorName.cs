using System;
using System.Linq;

namespace GridDomain.Node.AkkaMessaging
{
    public class AggregateActorName
    {
        private static readonly string Separator = "_";

        internal AggregateActorName(Type aggregateType, Guid id)
        {
            Id = id;
            AggregateType = aggregateType;
            Name = GetActorNameFromType(aggregateType) + Separator + id;
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
            var id = Guid.Parse(value.Replace(GetActorNameFromType(aggregateType) + Separator, ""));
            return new AggregateActorName(aggregateType, id);
        }

        public static string GetActorNameFromType(Type aggregateType)
        {
            if(!aggregateType.IsGenericType) 
                        return aggregateType.Name;

            var parameters = string.Join("_",
                aggregateType.GetGenericArguments().Select(GetActorNameFromType));
            var typeName = aggregateType.Name.Split('`')[0];
            
            return $"{typeName}_{parameters}";
        }

        public override string ToString()
        {
            return Name;
        }
    }


    public static class TypeNameExtensions
    {
        public static string BeautyName(this Type type)
        {
            return AggregateActorName.GetActorNameFromType(type);
        }   
    }
}