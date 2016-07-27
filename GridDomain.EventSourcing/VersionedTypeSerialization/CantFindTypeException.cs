using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class CantFindTypeException : Exception
    {
        public string OriginalTypeFullName { get;}

        public CantFindTypeException(string originalTypeFullName)
        {
            OriginalTypeFullName = originalTypeFullName;
        }
    }
}