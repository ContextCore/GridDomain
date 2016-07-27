using System.Runtime.Serialization.Formatters.Binary;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using NUnit.Framework;
using QuickGraph.Serialization;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    public class VersionedTypeNameTests
    {

        public class TestType
        {
            
        }

        public class TestType_V1
        {
            
        }

        public class BadNamedType_V1_V1
        {
            
        }
   

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


       //[Test]
       //public void Latest_version_is_resolved_as_original()
       //{
       //    var versionedType = VersionedTypeName.Parse("GridDomain.Tests.EventsUpgrade.VersionedTypeNameTests+TestType_V2");
       //    Assert.AreEqual(2, versionedType.Version);
       //    Assert.AreEqual(typeof(TestType).Name, versionedType.OriginalName);
       //}



    }
}