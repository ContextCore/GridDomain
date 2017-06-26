using System.Collections.Generic;
using Ploeh.AutoFixture;
using Shop.Infrastructure;
using Xunit;

namespace Shop.Tests.Unit
{
    public abstract class Sequence_provider_tests
    {
        protected abstract ISequenceProvider Provider { get; }
        protected readonly List<string> CreatedSequences = new List<string>();

        private string CreateSequenceName()
        {
            var sequenceName = "sequence" + new Fixture().Create<int>();
            CreatedSequences.Add(sequenceName);
            return sequenceName;
        }

        [Fact]
        public void Any_sequence_numbers_provided_in_increasing_sequence()
        {
            var sequenceName = CreateSequenceName();
            var numberA = Provider.GetNext(sequenceName);
            var numberB = Provider.GetNext(sequenceName);
            Assert.Equal(1, numberA);
            Assert.Equal(2, numberB);
        }

        [Fact]
        public void Default_number_default_sequence_is_one()
        {
            CreatedSequences.Add("global");
            var number = Provider.GetNext();
            Assert.Equal(1, number);
        }

        [Fact]
        public void Default_number_for_global_sequence_is_one()
        {
            CreatedSequences.Add("global");
            var number = Provider.GetNext("global");
            Assert.Equal(1, number);
        }

        [Fact]
        public void Default_number_for_random_sequence_is_one()
        {
            var sequenceName = CreateSequenceName();
            var number = Provider.GetNext(sequenceName);
            Assert.Equal(1, number);
        }

        [Fact]
        public void Default_sequence_sequence_is_global()
        {
            CreatedSequences.Add("global");
            var numberDefault = Provider.GetNext();
            var numberGlobal = Provider.GetNext("global");
            Assert.Equal(1, numberDefault);
            Assert.Equal(2, numberGlobal);
        }
    }
}