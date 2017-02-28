using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class CantFindTypeLatestVersionException : Exception
    {
        public CantFindTypeLatestVersionException(Type originalType)
        {
            OriginalType = originalType;
        }

        public Type OriginalType { get; }
    }
}