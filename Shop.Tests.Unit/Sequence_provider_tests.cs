using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Infrastructure;

namespace Shop.Tests.Unit
{
    [TestFixture]
    public abstract class Sequence_provider_tests 
    {
        protected abstract Func<ISequenceProvider> SequenceProviderFactory { get; }
        private ISequenceProvider _provider;
        protected readonly List<string> CreatedSequences = new List<string>();


        [SetUp]
        public void Given_new_sequence_provider()
        {
            _provider = SequenceProviderFactory();
        }


        [Test]
        public void Default_number_for_random_sequence_is_one()
        {
            var sequenceName = CreateSequenceName();
            var number = _provider.GetNext(sequenceName);
            Assert.AreEqual(1, number);
        }

        private string CreateSequenceName()
        {
            var sequenceName = "sequence" + new Fixture().Create<int>();
            this.CreatedSequences.Add(sequenceName);
            return sequenceName;
        }
 

        [Test]
        public void Default_number_for_global_sequence_is_one()
        {
            this.CreatedSequences.Add("global");
            var number = _provider.GetNext("global");
            Assert.AreEqual(1, number);
        }

        [Test]
        public void Default_number_default_sequence_is_one()
        {
            this.CreatedSequences.Add("global");
            var number = _provider.GetNext();
            Assert.AreEqual(1, number);
        }

        [Test]
        public void Default_sequence_sequence_is_global()
        {
            this.CreatedSequences.Add("global");
            var numberDefault = _provider.GetNext();
            var numberGlobal = _provider.GetNext("global");
            Assert.AreEqual(1, numberDefault);
            Assert.AreEqual(2, numberGlobal);
        }

        [Test]
        public void Any_sequence_numbers_provided_in_increasing_sequence()
        {
            var sequenceName = CreateSequenceName();
            var numberA = _provider.GetNext(sequenceName);
            var numberB = _provider.GetNext(sequenceName);
            Assert.AreEqual(1, numberA);
            Assert.AreEqual(2, numberB);
        }

    }
}