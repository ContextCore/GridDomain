using System;
using System.Collections.Generic;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Unit.EventsUpgrade.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    [TestFixture]
    public class VersionedTypeSerializationBinderTests
    {
        
        private Dictionary<Type, int> _knownTypeMaxVersions;
        private VersionedTypeSerializationBinder _binder;

        [OneTimeSetUp]
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
            var type = _binder.BindToType("GridDomain.Tests", "GridDomain.Tests.EventsUpgrade.Events.TestType_V2");
            Assert.AreEqual(typeof(TestType), type);
        }
    }
}
