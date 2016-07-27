using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class VersionedTypeParseExeption : Exception
    {
        public string TypeName { get;}

        public VersionedTypeParseExeption(string typeName) : base(typeName)
        {
            TypeName = typeName;
        }
    }
}