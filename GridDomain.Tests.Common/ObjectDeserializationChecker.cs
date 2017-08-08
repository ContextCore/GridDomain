using System;
using System.Text;
using Akka.Serialization;
using GridDomain.EventSourcing;
using KellermanSoftware.CompareNetObjects;

namespace GridDomain.Tests.Common
{
    public class ObjectDeserializationChecker
    {
        private readonly Serializer[] _serializer;

        public ObjectDeserializationChecker(CompareLogic logic, params Serializer[] serializer)
        {
            _serializer = serializer;
            CompareLogic = logic ?? new CompareLogic { Config = new ComparisonConfig() };
        }

        public CompareLogic CompareLogic { get; }
        public Action<object> AfterRestore { get; set; } = o => {};
        public bool IsRestorable(object original, out string difference)
        {
            bool isRestorable = true;
            var sb = new StringBuilder();
            foreach (var ser in _serializer)
            {
                string diff = null;
                isRestorable = isRestorable & IsRestorable(ser, original, out diff);
                if (string.IsNullOrEmpty(diff)) continue;

                sb.AppendLine($"checking by {ser.GetType()} results:");
                sb.AppendLine(diff);
            }

            difference = sb.ToString();
            return isRestorable;
        }

        private bool IsRestorable(Serializer serializer, object original, out string difference)
        {
            var bytes = serializer.ToBinary(original);
            var restored = serializer.FromBinary(bytes, original.GetType());
            AfterRestore(restored);
            var comparisonResult = CompareLogic.Compare(original, restored);
            difference = comparisonResult.AreEqual ? null : comparisonResult.DifferencesString;
            return comparisonResult.AreEqual;
        }
    }
}