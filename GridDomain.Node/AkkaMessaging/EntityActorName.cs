using System;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging
{
    public class EntityActorName
    {
        private const string Separator = "_";

        internal EntityActorName(string entityName, string id)
        {
            Id = id;
            Name = GetFullName(entityName, id);
        }

        public static string GetFullName(Type entityType, string id)
        {
            return GetFullName(entityType.BeautyName(), id);
        }
        
        public static string GetFullName(string entity, string id)
        {
            return entity + Separator + id;
        }
        
        public string Id { get; }

        public string Name { get; }

        public static EntityActorName New<T>(string id)
        {
            return New(typeof(T), id);
        }

        public static EntityActorName New(Type entityType, string id)
        {
            return new EntityActorName(entityType.BeautyName(), id);
        }

        public static EntityActorName Parse<T>(string value)
        {
            var entityType = typeof(T);
            var beautyName = entityType.BeautyName();
            var id = value.Replace(beautyName + Separator, "");
            return New(entityType, id);
        }

        public static bool TryParseId(string value, out string id)
        {
            var parts = value.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                id = parts[1];
                return true;
            }

            id = null;
            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}