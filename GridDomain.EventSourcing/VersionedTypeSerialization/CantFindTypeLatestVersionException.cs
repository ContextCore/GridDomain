using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class CantFindTypeLatestVersionException : Exception
    {
        public Type OriginalType { get;}

        public CantFindTypeLatestVersionException(Type originalType)
        {
            OriginalType = originalType;
        }
    }
}