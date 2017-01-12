using Akka.Serialization;
using GridDomain.EventSourcing;
using GridDomain.Node;
using KellermanSoftware.CompareNetObjects;

namespace GridDomain.Tests.Framework
{
    public class ObjectDeserializationChecker
    {
        private readonly DomainSerializer _serializer;
        private readonly CompareLogic _compareLogic;

        public CompareLogic CompareLogic => _compareLogic;

        public ObjectDeserializationChecker(DomainSerializer serializer = null, CompareLogic logic = null)
        {
            _serializer = serializer ?? new DomainSerializer();
            _compareLogic = logic ?? new CompareLogic {Config = new ComparisonConfig()};
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