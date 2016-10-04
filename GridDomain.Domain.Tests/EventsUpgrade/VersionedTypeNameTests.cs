using System.Runtime.Serialization.Formatters.Binary;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Events;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    public class VersionedTypeNameTests
    {
        [Test]
        public void Given_bad_exeption_is_raised()
        {
            Assert.Throws<VersionedTypeParseExeption>(() => VersionedTypeName.Parse(typeof(BadNamedType_V1_V1)));
        }

        [Test]
        public void Given_history_type_version_is_taken_from_type_name()
        {
            var versionedType = VersionedTypeName.Parse(typeof(TestType_V1));
            Assert.AreEqual(1,versionedType.Version);
        }

        [Test]
        public void Original_version_is_resolved_with_default_version()
        {
            var versionedType = VersionedTypeName.Parse(typeof(TestType),10);
            Assert.AreEqual(typeof(TestType).Name, versionedType.OriginalName);
            Assert.AreEqual(10, versionedType.Version);
        }
    }
}