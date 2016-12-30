using System;
using NUnit.Framework;
using Shop.Infrastructure;

namespace Shop.Tests.Unit
{
    [TestFixture]

    class InMemory_sequence_provider_tests : Sequence_provider_tests
    {
        protected override Func<ISequenceProvider> SequenceProviderFactory { get; } = () => new InMemorySequenceProvider();
    }
}