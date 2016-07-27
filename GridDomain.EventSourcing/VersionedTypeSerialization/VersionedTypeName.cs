using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class VersionedTypeName
    {
        private VersionedTypeName(string originalName, int version)
        {
            OriginalName = originalName;
            Version = version;
        }

        //public VersionedTypeName(Type type, int version):this(type.Name,version)
        //{
        //}

        public int Version { get; }
        public string OriginalName { get; }
        public static readonly string VersionSeparator = "_V";

        public override string ToString()
        {
            return $"{OriginalName}{VersionSeparator}{Version}";
        }

        public static VersionedTypeName Parse(Type type, int defaultVersion = 0)
        {
            return Parse(type.Name, defaultVersion);
        }
        public static VersionedTypeName Parse(string shortTypeName, int defaultVersion = 0)
        {
            var versionedTypeParts = shortTypeName
                .Split(new[] {VersionSeparator},StringSplitOptions.RemoveEmptyEntries);

            if (versionedTypeParts.Length == 1) //does not contain version
                return new VersionedTypeName(shortTypeName, defaultVersion);

            if (versionedTypeParts.Length != 2)
                throw new VersionedTypeParseExeption(shortTypeName);

            var typeName = versionedTypeParts[0];
            if (string.IsNullOrEmpty(typeName))
                throw new EmptyTypeNameException(versionedTypeParts);

            var versionString = versionedTypeParts[1];
            int version = 0;
            if (!Int32.TryParse(versionString, out version))
                throw new CantParseVersionNumberExpection(versionString);
          
            return new VersionedTypeName(typeName, version);
        }
    }
}