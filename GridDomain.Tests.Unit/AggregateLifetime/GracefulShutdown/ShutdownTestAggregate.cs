using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.AggregateLifetime.GracefulShutdown {
    public class ShutdownTestAggregate : ConventionAggregate
    {
        public static List<string> ExecutedCommands = new List<string>();
            
        public ShutdownTestAggregate(string id):base(id)
        {
            Execute<DoWorkCommand>(async c =>
                                   {
                                       if (c.Duration.HasValue)
                                           await Task.Delay(c.Duration.Value);
                                           
                                       Produce(new WorkDone(Id, c.Parameter));
                                   });
            Apply<WorkDone>(e => ExecutedCommands.Add(e.Value));
        }
    }
}