using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain
{
    public class Balloon : Aggregate
    {
        private Balloon(Guid id) : base(id) {}

        public Balloon(Guid id, string value) : this(id)
        {
            Emit(new BalloonCreated(value, id));
        }

        public string Title { get; private set; }

        public static Balloon FromSnapshot(IMemento memento)
        {
            var snapshot = memento as BalloonSnapshot;
            if (snapshot == null)
                throw new InvalidOperationException("Sample aggregate can be restored only from memento with type "
                                                    + typeof(BalloonSnapshot).Name);

            var aggregate = new Balloon(snapshot.Id, snapshot.Value);
            aggregate.PersistAll();
            aggregate.Version = snapshot.Version;
            return aggregate;
        }

        public override IMemento GetSnapshot()
        {
            return new BalloonSnapshot(Id, Version, Title);
        }

        public void WriteNewTitle(int number)
        {
           // Emit(Task.Delay(1000).ContinueWith(t => new BalloonTitleChanged(number.ToString(), Id)));
          // Thread.Sleep(1000);
           Emit(new BalloonTitleChanged(number.ToString(), Id));
        }

        public void InflateNewBaloon(string value)
        {
            Emit(new BalloonCreated(value, Id),
                 new BalloonTitleChanged(value, Id));
        }

        public void IncreaseTitle(int value)
        {
            Emit(new BalloonTitleChanged((value + int.Parse(Title)).ToString(), Id));
        }

        public async Task PlanTitleWrite(int sleepMiliseconds)
        {
            var eventTask = Task.Delay(sleepMiliseconds).
                                 ContinueWith(t => new BalloonTitleChanged(sleepMiliseconds.ToString(), Id));

            await Emit(eventTask);

            var evtTask2 = await Task.Delay(sleepMiliseconds)
                                     .ContinueWith(t => new BalloonTitleChanged(sleepMiliseconds.ToString(), Id));
            Emit(evtTask2);
        }

        internal async Task PlanTitleWrite(int parameter, TimeSpan sleepTime)
        {
            await Task.Delay(sleepTime);
            Emit(new BalloonTitleChanged(parameter.ToString(), Id));
        }

        internal async Task PlanWriteTitleToBlow(int parameter, TimeSpan sleepTime)
        {
            Emit(await Task.Delay(sleepTime)
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
            Emit(new BalloonTitleChanged("0", Id));
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