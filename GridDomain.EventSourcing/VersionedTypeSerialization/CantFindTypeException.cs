using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class CantFindTypeException : Exception
    {
        public CantFindTypeException()
        {
            
        }
        public CantFindTypeException(string originalTypeFullName)
        {
            OriginalTypeFullName = originalTypeFullName;
        }

        public string OriginalTypeFullName { get; }
    }
}