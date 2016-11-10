using Akka.Serialization;
using GridDomain.EventSourcing;
using GridDomain.Node;
using KellermanSoftware.CompareNetObjects;

namespace GridDomain.Tests.Framework
{
    class ObjectDeserializationChecker
    {
        private readonly WireJsonSerializer _serializer;
        private readonly CompareLogic _compareLogic;

        public ObjectDeserializationChecker(WireJsonSerializer serializer = null, CompareLogic logic = null)
        {
            _serializer = serializer ?? new WireJsonSerializer();
            _compareLogic = logic ?? new CompareLogic {Config = new ComparisonConfig() {ComparePrivateFields = true}};
        }

        public bool IsRestorable(object original, out string difference)
        {
            var bytes = _serializer.ToBinary(original);
            var restored = _serializer.FromBinary(bytes, original.GetType());

            var comparisonResult = _compareLogic.Compare(original, restored);

            difference = comparisonResult.AreEqual ? null : comparisonResult.DifferencesString;
            return comparisonResult.AreEqual;
        }

    }
}