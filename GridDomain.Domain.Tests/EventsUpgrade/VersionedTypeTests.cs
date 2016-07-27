using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using NUnit.Framework;

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

        //[TestFixtureSetUp]
        //private Dictionary<Type, int> _knownTypeMaxVersions;
        //public void Given_VersionedTypeSerializationBinder()
        //{
        //   // _knownTypeMaxVersions = new Dictionary<Type,int>
        //   // {
        //   //     {typeof(TestType),1}
        //   // };
        //   // var binder = new Versioned
        //}

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
        public void Original_version_is_resolved_as_is()
        {
            var versionedType = VersionedTypeName.Parse(typeof(TestType));
            Assert.AreEqual(0, versionedType.Version);
            Assert.AreEqual(typeof(TestType).Name, versionedType.OriginalName);
        }

        //[Test]
        //public void Historical_version_is_resolved_as_type_with_version()
        //{

        //}
    }
}
