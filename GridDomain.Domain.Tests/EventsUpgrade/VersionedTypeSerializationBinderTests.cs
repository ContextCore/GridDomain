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
    public class VersionedTypeSerializationBinderTests
    {
        
        public class TestType
        {

        }

        public class TestType_V1
        {

        }

        private Dictionary<Type, int> _knownTypeMaxVersions;
        private VersionedTypeSerializationBinder _binder;

        [TestFixtureSetUp]
        public void Given_VersionedTypeSerializationBinder()
        {
            _knownTypeMaxVersions = new Dictionary<Type, int>
             {
                 {typeof(TestType),2}
             };
            _binder = new VersionedTypeSerializationBinder(_knownTypeMaxVersions);
        }

        [Test]
        public void Historical_version_is_resolved_as_type_with_version()
        {
            var fullTypeName = typeof(TestType).FullName;

            var typeTry = Type.GetType(fullTypeName);
            var type = _binder.BindToType("GridDomain.Tests", "GridDomain.Tests.EventsUpgrade.VersionedTypeSerializationBinderTests+TestType_V1");
            Assert.AreEqual(typeof(TestType_V1),type);
        }

        [Test]
        public void Latest_version_is_resolved_as_original_type()
        {
            var type = _binder.BindToType("GridDomain.Tests", "GridDomain.Tests.EventsUpgrade.VersionedTypeSerializationBinderTests+TestType_V2");
            Assert.AreEqual(typeof(TestType), type);
        }
    }
}
