using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.Unit.EventsUpgrade.Events;
using Xunit;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class VersionedTypeNameTests
    {
        [Fact]
        public void Given_bad_exeption_is_raised()
        {
            Assert.Throws<VersionedTypeParseExeption>(() => VersionedTypeName.Parse(typeof(BadNamedType_V1_V1)));
        }

        [Fact]
        public void Given_history_type_version_is_taken_from_type_name()
        {
            var versionedType = VersionedTypeName.Parse(typeof(TestType_V1));
            Assert.Equal(1, versionedType.Version);
        }

        [Fact]
        public void Original_version_is_resolved_with_default_version()
        {
            var versionedType = VersionedTypeName.Parse(typeof(TestType), 10);
            Assert.Equal(typeof(TestType).Name, versionedType.OriginalName);
            Assert.Equal(10, versionedType.Version);
        }
    }
}