using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Aggregates.Abstractions;
using Xunit;

namespace GridDomain.Aggregates.Tests
{
    public class AggregateAddressTests
    {
        class MyAggregate : IAggregate
        {
            public string Id { get; }
            public void Apply(IDomainEvent @event)
            {
                throw new System.NotImplementedException();
            }

            public long Version { get; }
            public Task<IReadOnlyCollection<IDomainEvent>> Execute(ICommand command)
            {
                throw new System.NotImplementedException();
            }
        }
        
        
        [Theory]
        [InlineData("MyAggregate_123_123","MyAggregate","123_123")]
        [InlineData("B_ecd","B","ecd")]
        [InlineData("B_ecd_","B","ecd_")]
        [InlineData("A_123","A","123")]
        public void ShouldParseFromString(string rawString, string expectedName, string expectedId)
        {
            var address = AggregateAddress.Parse(rawString);
            Assert.Equal(expectedName,address.Name);
            Assert.Equal(expectedId,address.Id);
        }
        
        [Theory]
        [InlineData("MyAggregate")]
        [InlineData("_123")]
        [InlineData("_123_")]
        [InlineData("123_")]
        public void ShouldThrowException(string rawString)
        {
            Assert.Throws<AggregateAddress.BadAggregateAddressFormatException>(()=>AggregateAddress.Parse(rawString));
        }
        
       
    }
}