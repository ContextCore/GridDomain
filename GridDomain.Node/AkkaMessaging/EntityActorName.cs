using System;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging
{
    public class EntityActorName
    {
        private static readonly string Separator = ":";

        internal EntityActorName(string entityName, Guid id)
        {
            Id = id;
            Name = entityName + Separator + id;
        }

        public Guid Id { get; }

        public string Name { get; }

        public static EntityActorName New<T>(Guid id)
        {
            return New(typeof(T), id);
        }

        public static EntityActorName New(Type entityType, Guid id)
        {
            return new EntityActorName(entityType.BeautyName(), id);
        }

        public static EntityActorName Parse<T>(string value)
        {
            var entityType = typeof(T);
            var beautyName = entityType.BeautyName();
            var id = Guid.Parse(value.Replace(beautyName + Separator, ""));
            return New(entityType, id);
        }

        public static bool TryParseId(string value, out Guid id)
        {
            var parts = value.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);

            return Guid.TryParse(parts[1], out id);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}