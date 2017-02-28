using GridDomain.EventSourcing;
using KellermanSoftware.CompareNetObjects;

namespace GridDomain.Tests.Framework
{
    public class ObjectDeserializationChecker
    {
        private readonly DomainSerializer _serializer;

        public ObjectDeserializationChecker(DomainSerializer serializer = null, CompareLogic logic = null)
        {
            _serializer = serializer ?? new DomainSerializer();
            CompareLogic = logic ?? new CompareLogic {Config = new ComparisonConfig()};
        }

        public CompareLogic CompareLogic { get; }

        public bool IsRestorable(object original, out string difference)
        {
            var bytes = _serializer.ToBinary(original);
            var restored = _serializer.FromBinary(bytes, original.GetType());

            var comparisonResult = CompareLogic.Compare(original, restored);

            difference = comparisonResult.AreEqual ? null : comparisonResult.DifferencesString;
            return comparisonResult.AreEqual;
        }
    }
}