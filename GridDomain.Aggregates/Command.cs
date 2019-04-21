using System;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public class Command : ICommand
    {

        protected Command(string id, IAggregateAddress recipient, DateTime? time=null)
        {
            Id = id;
            Time = time ?? DateTimeOffset.Now;
            Recipient = recipient;
        }

      
        protected Command(IAggregateAddress aggregate, DateTime? time=null) : this(Guid.NewGuid().ToString(), aggregate, time) {}

        public DateTimeOffset Time { get; private set; }
        public string Id { get; private set; }
       
        public IAggregateAddress Recipient { get; private set; }
    }

    public class Command<T> : Command where T: class, IAggregate 
    {
        protected Command(string aggregateId):base(aggregateId.AsAddressFor<T>()){}
    }
}