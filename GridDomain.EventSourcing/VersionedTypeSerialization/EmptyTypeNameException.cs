using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    internal class EmptyTypeNameException : Exception
    {
        public string[] VersionedTypeParts { get; }

        public EmptyTypeNameException(string[] versionedTypeParts)
        {
            VersionedTypeParts = versionedTypeParts;
        }
    }
}