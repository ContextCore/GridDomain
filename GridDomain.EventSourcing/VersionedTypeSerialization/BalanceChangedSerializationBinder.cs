using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    /// <summary>
    /// How to updata and event
    /// 1) Create a copy of event and add existing number in type by convention _V(N) where N is version
    /// for example BalanceAggregateCreatedEvent should be copied as BalanceAggregateCreatedEvent_V1.
    /// All existing persisted events must be convertible to versioned one by duck typing. 
    /// 2) Update existing event. 
    ///    Scenarios: 
    ///    a) Add field
    ///    b) Remove field
    ///    c) Change field name
    ///    d) Change field type
    ///    e) Rename event
    ///    f) Event splitting
    ///    
    /// 3) Create an event adapter from versioned type to new one 
    /// 4) Register event adapter 
    /// </summary>

 
    public class VersionedTypeSerializationBinder : SerializationBinder
    {
        private readonly IDictionary<Type, int> _typeMaxVersions;

        public VersionedTypeSerializationBinder(IDictionary<Type, int> typeMaxVersions )
        {
            _typeMaxVersions = typeMaxVersions;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var versionedTypeName = VersionedTypeName.Parse(typeName);
            var originalTypeFullName = $"{versionedTypeName.OriginalName}{Type.Delimiter}{assemblyName}";
            var originalType = Type.GetType(originalTypeFullName,false);
            if (originalType == null)
                throw new CantFindTypeException(originalTypeFullName);

            int typeMaxVersion = 0;
            if (!_typeMaxVersions.TryGetValue(originalType, out typeMaxVersion))
                throw new CantFindTypeLatestVersionException(originalType);

            //if max version is reached, should return type without version number
            //because it is most recent version used in code now
            if (typeMaxVersion == versionedTypeName.Version)
                return originalType;

            //otherwise return original type, as it acts like "history"
            originalTypeFullName = $"{typeName}{Type.Delimiter}{assemblyName}";
            return Type.GetType(originalTypeFullName);
        }
    }
}