using System.Diagnostics;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders
{
    public class AggregateCreatedProjectionBuilder_Alternative : IHandler<BalloonCreated>
    {
        private static readonly Stopwatch Watch = new Stopwatch();

        static AggregateCreatedProjectionBuilder_Alternative()
        {
            Watch.Start();
        }

        public Task Handle(BalloonCreated msg)
        {
            msg.History.SequenceNumber = int.Parse(msg.Value);
            msg.History.ElapsedTicksFromAppStart = Watch.ElapsedTicks;
            return Task.CompletedTask;
        }
    }
}