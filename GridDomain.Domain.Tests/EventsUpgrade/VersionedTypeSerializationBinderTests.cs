using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing.DomainEventAdapters;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Events;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    public class VersionedTypeSerializationBinderTests
    {
        
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
            var type = _binder.BindToType("GridDomain.Tests", typeof(TestType_V1).FullName);
            Assert.AreEqual(typeof(TestType_V1),type);
        }

        [Test]
        public void Latest_version_is_resolved_as_original_type()
        {
            var type = _binder.BindToType("GridDomain.Tests", "GridDomain.Tests.EventsUpgrade.TestType_V2");
            Assert.AreEqual(typeof(TestType), type);
        }
    }
}
