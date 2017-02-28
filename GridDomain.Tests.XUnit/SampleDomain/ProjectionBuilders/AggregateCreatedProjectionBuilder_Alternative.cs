using System.Diagnostics;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
{
    public class AggregateCreatedProjectionBuilder_Alternative : IHandler<SampleAggregateCreatedEvent>
    {
        private static readonly Stopwatch Watch = new Stopwatch();

        static AggregateCreatedProjectionBuilder_Alternative()
        {
            Watch.Start();
        }

        public Task Handle(SampleAggregateCreatedEvent msg)
        {
            msg.History.SequenceNumber = int.Parse(msg.Value);
            msg.History.ElapsedTicksFromAppStart = Watch.ElapsedTicks;
            return Task.CompletedTask;
        }
    }
}