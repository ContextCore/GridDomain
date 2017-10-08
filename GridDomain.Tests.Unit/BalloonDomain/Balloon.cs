using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools;

namespace GridDomain.Tests.Unit.BalloonDomain
{
    public class Balloon : Aggregate
    {
        private Balloon(Guid id) : base(id) {}

        public Balloon(Guid id, string value) : this(id)
        {
            Produce(new BalloonCreated(value, id));
        }

        public string Title { get; private set; }

        public static Balloon FromSnapshot(IMemento memento)
        {
            var snapshot = memento as BalloonSnapshot;
            if (snapshot == null)
                throw new InvalidOperationException("Sample aggregate can be restored only from memento with type "
                                                    + typeof(BalloonSnapshot).Name);

            var aggregate = new Balloon(snapshot.Id, snapshot.Value);
            aggregate.MarkAllPesisted();
            aggregate.Version = snapshot.Version;
            return aggregate;
        }

        public override IMemento GetSnapshot()
        {
            return new BalloonSnapshot(Id, Version, Title);
        }

        public void WriteNewTitle(int number)
        {
            Produce(new BalloonTitleChanged(number.ToString(), Id));
        }

        public void InflateNewBaloon(string value)
        {
            Produce(new BalloonCreated(value, Id),
                 new BalloonTitleChanged(value, Id));
        }

        public void IncreaseTitle(int value)
        {
            Produce(new BalloonTitleChanged((value + int.Parse(Title)).ToString(), Id));
        }
        //demo for actions depending on changed state after emit
        public async Task DoubleIncreaseTitle(int value)
        {
            await Emit(new BalloonTitleChanged(value.ToString(), Id));
            //should be changed value from previous emit
            var newValue = int.Parse(Title) + 1;
            Produce(new BalloonTitleChanged((newValue).ToString(), Id));
        }
        public async Task PlanTitleWrite(int sleepMiliseconds)
        {
            var eventTask = Task.Delay(sleepMiliseconds).
                                 ContinueWith(t => new BalloonTitleChanged(sleepMiliseconds.ToString(), Id));

            await Emit(eventTask);
        }

        internal async Task PlanTitleWrite(int parameter, TimeSpan sleepTime)
        {
            await Task.Delay(sleepTime);
            Produce(new BalloonTitleChanged(parameter.ToString(), Id));
        }

        internal async Task PlanWriteTitleToBlow(int parameter, TimeSpan sleepTime)
        {
            Produce(await Task.Delay(sleepTime)
                                    .ContinueWith(t =>
                                    {
                                        Blow();
                                        return new BalloonTitleChanged(parameter.ToString(), Id);
                                    }));
        }

        private void Apply(BalloonCreated e)
        {
            Id = e.SourceId;
            Title = e.Value;
        }

        private void Apply(BalloonTitleChanged e)
        {
            Title = e.Value;
        }

        public void Blow()
        {
            throw new BalloonException();
        }

        public async Task BlowAfter(TimeSpan callBackTime)
        {
            await Task.Delay(callBackTime);
            Blow();
            Produce(new BalloonTitleChanged("0", Id));
        }

        private class BalloonSnapshot : IMemento
        {
            public BalloonSnapshot(Guid id, int version, string value)
            {
                Id = id;
                Version = version;
                Value = value;
            }

            public string Value { get; }

            public Guid Id { get; set; }
            public int Version { get; set; }
        }
    }
}