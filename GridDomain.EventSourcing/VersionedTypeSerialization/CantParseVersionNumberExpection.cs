using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    internal class CantParseVersionNumberExpection : Exception
    {
        public string VersionString { get; set; }

        public CantParseVersionNumberExpection(string versionString)
        {
            VersionString = versionString;
        }
    }
}