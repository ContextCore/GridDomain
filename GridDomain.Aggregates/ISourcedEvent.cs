using System;

namespace GridDomain.Aggregates
{
    /// <summary>
    ///     Represents an event message.
    /// </summary>
    public interface ISourcedEvent
    {
        /// <summary>
        ///     Gets the identifier of the source originating the event.
        /// </summary>
        string SourceId { get; }
        DateTime CreatedTime { get; }
    }


    public class AggregateDependencies<TAggregate>:IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        public AggregateDependencies(IAggregateFactory<TAggregate> factory=null)
        {
            if(factory!= null)
                AggregateFactory = factory;
        }
        public IAggregateFactory<TAggregate> AggregateFactory { get; } = GridDomain.Aggregates.AggregateFactory.For<TAggregate>();
        public IAggregateConfiguration Configuration { get;  } = new AggregateConfiguration();
    }

    public class AggregateConfiguration : IAggregateConfiguration
    {
        public TimeSpan MaxInactivityPeriod { get; } = TimeSpan.FromMinutes(30);
        public int SnapshotsKeepAmount { get; } = 5;
    }
    
    public interface IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        IAggregateFactory<TAggregate> AggregateFactory { get; }
        IAggregateConfiguration Configuration { get; }
    }

    public interface IAggregateConfiguration
    {
        TimeSpan MaxInactivityPeriod { get; }
        int SnapshotsKeepAmount { get; }
    }
   
}