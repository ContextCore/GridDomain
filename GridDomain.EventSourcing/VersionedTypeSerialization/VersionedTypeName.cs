using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class VersionedTypeName
    {
        public VersionedTypeName(string originalName, int version)
        {
            OriginalName = originalName;
            Version = version;
        }

        public int Version { get; }
        public string OriginalName { get; }
        public static readonly string VersionSeparator = "_V";

        public override string ToString()
        {
            return $"{OriginalName}{VersionSeparator}{Version}";
        }

        public static VersionedTypeName New<T>(int version)
        {
            return new VersionedTypeName(typeof(T).Name, version);
        }

        public static VersionedTypeName Parse(Type type)
        {
            return Parse(type.Name);
        }
        public static VersionedTypeName Parse(string shortTypeName)
        {
            var versionedTypeParts = shortTypeName
                .Split(new[] {VersionSeparator},StringSplitOptions.RemoveEmptyEntries);

            if (versionedTypeParts.Length == 1) //does not contain version
                return new VersionedTypeName(shortTypeName, 0);

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