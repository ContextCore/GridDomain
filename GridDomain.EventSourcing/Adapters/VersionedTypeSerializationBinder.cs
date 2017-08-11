using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using Newtonsoft.Json;

namespace GridDomain.EventSourcing.Adapters
{
    public class VersionedTypeSerializationBinder : SerializationBinder
    {
        private readonly IDictionary<Type, int> _typeMaxVersions;

        public VersionedTypeSerializationBinder(IDictionary<Type, int> typeMaxVersions)
        {
            _typeMaxVersions = typeMaxVersions;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var versionedTypeName = VersionedTypeName.Parse(typeName);
            var originalTypeFullName = $"{versionedTypeName.OriginalName}, {assemblyName}";
            var originalType = Type.GetType(originalTypeFullName, false);
            if (originalType == null)
                throw new CantFindTypeException(originalTypeFullName);

            var typeMaxVersion = 0;
            if (!_typeMaxVersions.TryGetValue(originalType, out typeMaxVersion))
                throw new CantFindTypeLatestVersionException(originalType);

            //if max version is reached, should return type without version number
            //because it is most recent version used in code now
            if (typeMaxVersion == versionedTypeName.Version)
                return originalType;

            //otherwise return original type, as it acts like "history"
            originalTypeFullName = $"{typeName}, {assemblyName}";
            return Type.GetType(originalTypeFullName);
        }
    }
}