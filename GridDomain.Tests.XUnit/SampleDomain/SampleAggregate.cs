using System;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain
{
    public class SampleAggregate : Aggregate
    {
        private SampleAggregate(Guid id) : base(id) {}

        public SampleAggregate(Guid id, string value) : this(id)
        {
            RaiseEvent(new SampleAggregateCreatedEvent(value, id));
        }

        public string Value { get; private set; }

        public static SampleAggregate FromSnapshot(IMemento memento)
        {
            var snapshot = memento as Snapshot;
            if (snapshot == null)
                throw new InvalidOperationException("Sample aggregate can be restored only from memento with type "
                                                    + typeof(Snapshot).Name);

            var aggregate = new SampleAggregate(snapshot.Id, snapshot.Value) {Version = snapshot.Version};
            (aggregate as IAggregate).ClearUncommittedEvents();
            return aggregate;
        }

        protected override IMemento GetSnapshot()
        {
            return new Snapshot(Id, Version, Value);
        }

        public void ChangeState(int number)
        {
            Emit(new SampleAggregateChangedEvent(number.ToString(), Id));
        }

        public void CreateAndChangeState(string value)
        {
             Emit(new SampleAggregateCreatedEvent(value, Id),
                        new SampleAggregateChangedEvent(value, Id));
        }

        public void IncreaseParameter(int value)
        {
             Emit(new SampleAggregateChangedEvent((value + int.Parse(Value)).ToString(), Id));
        }

        public void LongExecute(int sleepMiliseconds)
        {
            var eventTask = Task.Delay(sleepMiliseconds)
                                .ContinueWith(t => new SampleAggregateChangedEvent(sleepMiliseconds.ToString(), Id));

             Emit(eventTask);
        }

        internal void ChangeStateAsync(int parameter, TimeSpan sleepTime)
        {
             Emit(Task.Delay(sleepTime)
                      .ContinueWith(t => new SampleAggregateChangedEvent(parameter.ToString(), Id)));
        }

        internal void AsyncExceptionWithOneEvent(int parameter, TimeSpan sleepTime)
        {
            Emit(Task.Delay(sleepTime)
                     .ContinueWith(t => new SampleAggregateChangedEvent(parameter.ToString(), Id)),
                 e => RaiseException());

        }

        private void Apply(SampleAggregateCreatedEvent e)
        {
            Id = e.SourceId;
            Value = e.Value;
        }

        private void Apply(SampleAggregateChangedEvent e)
        {
            Value = e.Value;
        }

        public void RaiseException()
        {
            throw new SampleAggregateException();
        }

        public void RaiseExceptionAsync(TimeSpan callBackTime)
        {
             Emit(Task.Delay(callBackTime)
                      .ContinueWith(t => new SampleAggregateChangedEvent("0", Id)),
                  t => RaiseException());
        }

        private class Snapshot : IMemento
        {
            public Snapshot(Guid id, int version, string value)
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