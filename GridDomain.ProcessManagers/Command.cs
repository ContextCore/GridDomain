using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.ProcessManagers {
    public class Command<T> : Command where T:IAggregate 
    {
        protected Command(string id, string aggregateId, string processId, DateTime time) : base(id, aggregateId, processId, time,typeof(T).BeautyName()) { }
        protected Command(string id, string aggregateId, string processId) : base(id, aggregateId, processId,typeof(T).BeautyName()) { }
        protected Command(string id, string aggregateId, DateTime time) : base(id, aggregateId, time,typeof(T).BeautyName()) { }
        protected Command(string id, string aggregateId) : base(id, aggregateId, typeof(T).BeautyName()) { }
        protected Command(string aggregateId) : base(aggregateId, typeof(T).BeautyName()) { }
    }
}