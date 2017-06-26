using System.Diagnostics;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders
{
    public class AggregateChangedProjectionBuilder : IHandler<BalloonTitleChanged>
    {
        private static readonly Stopwatch Watch = new Stopwatch();

        static AggregateChangedProjectionBuilder()
        {
            Watch.Start();
        }

        public virtual Task Handle(BalloonTitleChanged msg)
        {
            msg.History.SequenceNumber = int.Parse(msg.Value);
            msg.History.ElapsedTicksFromAppStart = Watch.ElapsedTicks;
            return Task.CompletedTask;
        }
    }
}