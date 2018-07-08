using System.Linq;
using Autofac.Core;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Node.Cluster {
    public class ClusterAggregateConfiguration<TActor,TAggregate> : AggregateConfiguration<TActor, TAggregate> where TAggregate : Aggregate
    {
        public ClusterAggregateConfiguration(IAggregateDependencyFactory<TAggregate> factory) : base(factory) { }

        protected override Parameter[] CreateParametersRegistration()
        {
            return base.CreateParametersRegistration()
                       .Union(new[]
                              {
                                  new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IRecycleConfiguration),
                                                        (pi, ctx) => AggregateDependencyFactory.CreateRecycleConfiguration())
                              })
                       .ToArray();
        }
    }
}