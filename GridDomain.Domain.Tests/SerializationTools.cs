using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Akka.Serialization;
using GridDomain.Node;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests
{
    [TestFixture]
    class ObjectDeserializationChecker
    {
        private readonly Serializer _serializer;
        private readonly CompareLogic _compareLogic;

        public ObjectDeserializationChecker(Serializer serializer = null, CompareLogic logic = null)
        {
            _serializer = serializer ?? new DomainEventsJsonSerializer(null);
            _compareLogic = logic ?? new CompareLogic { Config = new ComparisonConfig() { ComparePrivateFields = true } };
        }
        public bool IsRestorable(object original, out string difference)
        {
            var bytes = _serializer.ToBinary(original);
            var restored = _serializer.FromBinary(bytes,original.GetType());

            var comparisonResult = _compareLogic.Compare(original, restored);

            difference = comparisonResult.AreEqual ? null : comparisonResult.DifferencesString;
            return comparisonResult.AreEqual;
        }
    }

    public abstract class Type_should_be_deserializable
    {
        protected abstract object GetObject();

        [Test]
        public void Object_should_be_serializable_and_deserializable()
        {
            var checker = new ObjectDeserializationChecker();

            string differenceString;
            var isRestorable = checker.IsRestorable(GetObject(),out differenceString);
            Assert.True(isRestorable, differenceString);
        }
    }


    [TestFixture]
    public class All_types_inherited_from_base_should_be_deserializable
    {
        public void CheckAllChildrenOf<T>(Assembly assembly, params object[] objects)
        {
            var objectsCache = objects.ToDictionary(o => o.GetType(), o => o);
            var typesToCheck = assembly.GetTypes().Where(t => typeof(T).IsAssignableFrom(t));
            var fixture = new Fixture();

            //fixture.Build<>

        }
    }
}