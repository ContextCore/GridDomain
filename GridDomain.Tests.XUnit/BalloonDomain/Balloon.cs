using System;
using System.Threading;
using System.Threading.Tasks;

using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.XUnit.BalloonDomain.Events;

namespace GridDomain.Tests.XUnit.BalloonDomain
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

        public void PlanTitleWrite(int sleepMiliseconds)
        {
            var eventTask = Task.Delay(sleepMiliseconds)
                                .ContinueWith(t => new BalloonTitleChanged(sleepMiliseconds.ToString(), Id));

            Emit(eventTask);
        }

        internal void PlanTitleWrite(int parameter, TimeSpan sleepTime)
        {
            Emit(Task.Delay(sleepTime)
                     .ContinueWith(t => new BalloonTitleChanged(parameter.ToString(), Id)));
        }

        internal void PlanWriteTitleToBlow(int parameter, TimeSpan sleepTime)
        {
            Emit(Task.Delay(sleepTime)
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

        public void BlowAfter(TimeSpan callBackTime)
        {
            Emit(Task.Delay(callBackTime)
                     .ContinueWith(t =>
                                   {
                                       Blow();
                                       return new BalloonTitleChanged("0", Id);
                                   })
                );
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