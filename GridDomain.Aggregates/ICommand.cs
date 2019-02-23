using System;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public interface ICommand : IHaveId
    {
       // string ProcessId { get; set; }
        string AggregateId { get; }
        Type AggregateType { get; }
    }


    public interface ICommand<T> : ICommand, IFor<T>
    {
        
    }


    public class Command<T> : Command, ICommand<T> where T: class 
    {
        protected Command(string id, string aggregateId, string processId, DateTime time) : base(id, aggregateId, processId, time,typeof(T)) { }
        protected Command(string id, string aggregateId, string processId) : base(id, aggregateId, processId,typeof(T)) { }
        protected Command(string id, string aggregateId, DateTime time) : base(id, aggregateId, time,typeof(T)) { }
        protected Command(string id, string aggregateId) : base(id, aggregateId, typeof(T)) { }
        protected Command(string aggregateId) : base(aggregateId, typeof(T)) { }
    }
}