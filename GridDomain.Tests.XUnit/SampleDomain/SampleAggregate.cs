using System;
using System.Threading;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain
{
    public class SampleAggregate : Aggregate
    {
        private class Snapshot : IMemento
        {
            public Snapshot(Guid id, int version, string value)
            {
                Id = id;
                Version = version;
                Value = value;
            }

            public Guid Id { get; set; }
            public int Version { get; set; }
            public string Value { get; }
        }

        public static SampleAggregate FromSnapshot(IMemento memento)
        {
            var snapshot = memento as Snapshot;
            if(snapshot == null)
                throw new InvalidOperationException("Sample aggregate can be restored only from memento with type " + typeof(Snapshot).Name );

            var aggregate = new SampleAggregate(snapshot.Id, snapshot.Value) {Version = snapshot.Version};
            (aggregate as IAggregate).ClearUncommittedEvents();
            return aggregate;
        }

        protected override IMemento GetSnapshot()
        {
            return new Snapshot(this.Id, this.Version, Value);
        }

        public string Value { get; private set; }

        private SampleAggregate(Guid id) : base(id)
        {
            
        }

        public SampleAggregate(Guid id, string value):this(id)
        {
            RaiseEvent(new SampleAggregateCreatedEvent(value,id));
        }

        public void ChangeState(int number)
        {
            RaiseEvent(new SampleAggregateChangedEvent(number.ToString(), Id));
        }

        public void CreateAndChangeState(string value)
        {
            RaiseEvent(new SampleAggregateCreatedEvent(value, Id));
            RaiseEvent(new SampleAggregateChangedEvent(value, Id));
        }

        public void LongExecute(int sleepMiliseconds)
        {
            Thread.Sleep(sleepMiliseconds);
            ChangeState(sleepMiliseconds);
        }

        private Task<DomainEvent[]> CreateEventsTask(int param, TimeSpan sleepTime)
        {
            var timeSpan = sleepTime;
            var eventTask = Task.Run(() =>
            {
                Thread.Sleep(timeSpan);
                return new DomainEvent[] { new SampleAggregateChangedEvent(param.ToString(), Id)};
            });
            return eventTask;
        }

        private Task<SampleAggregateChangedEvent> CreateEventTask(int param, TimeSpan sleepTime)
        {
            var timeSpan = sleepTime;
            var eventTask = Task.Run(() =>
            {
                Thread.Sleep(timeSpan);
                return new SampleAggregateChangedEvent(param.ToString(), Id);
            });
            return eventTask;
        }


        internal void ChangeStateAsync(int parameter, TimeSpan sleepTime)
        {
            var eventTask = CreateEventsTask(parameter,sleepTime);
            RaiseEventAsync(eventTask);
        }

        internal void AsyncExceptionWithOneEvent(int parameter, TimeSpan sleepTime)
        {
            var expectionTask = CreateEventTask(0, sleepTime).ContinueWith(
             t =>
             {
                 RaiseException();
                 return t.Result;
             });
            RaiseEventAsync(expectionTask);
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
         var expectionTask = CreateEventsTask(0,callBackTime).ContinueWith(
             t =>
             {
                 RaiseException();
                 return t.Result;
             });

            RaiseEventAsync(expectionTask);
        }
    }
}