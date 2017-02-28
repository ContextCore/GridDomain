using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    internal class CantParseVersionNumberExpection : Exception
    {
        public CantParseVersionNumberExpection(string versionString)
        {
            VersionString = versionString;
        }

        public string VersionString { get; set; }
    }
}