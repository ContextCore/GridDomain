using System;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public interface ICommand : IHaveId
    {
       // string ProcessId { get; set; }
        string AggregateId { get; }
        string AggregateName { get; }
    }


    public interface ICommand<T> : ICommand, IFor<T>
    {
        
    }


    public class Command<T> : Command where T: class 
    {
        protected Command(string id, string aggregateId, string processId, DateTime time) : base(id, aggregateId, processId, time,typeof(T).BeautyName()) { }
        protected Command(string id, string aggregateId, string processId) : base(id, aggregateId, processId,typeof(T).BeautyName()) { }
        protected Command(string id, string aggregateId, DateTime time) : base(id, aggregateId, time,typeof(T).BeautyName()) { }
        protected Command(string id, string aggregateId) : base(id, aggregateId, typeof(T).BeautyName()) { }
        protected Command(string aggregateId) : base(aggregateId, typeof(T).BeautyName()) { }
    }
}