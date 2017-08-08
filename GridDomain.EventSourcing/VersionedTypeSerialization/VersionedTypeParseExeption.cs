using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class VersionedTypeParseExeption : Exception
    {
        public VersionedTypeParseExeption(string typeName) : base(typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; }
    }
}