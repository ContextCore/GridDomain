using Shop.Infrastructure;

namespace Shop.Tests.Unit.XUnit
{
    public class InMemory_sequence_provider_tests : Sequence_provider_tests
    {
        public InMemory_sequence_provider_tests()
        {
            Provider = new InMemorySequenceProvider();
        }

        protected override ISequenceProvider Provider { get; }
    }
}