using System.Linq;
using Autofac.Core;
using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Cluster {
    public class ClusterProcessConfiguration<TState, TActor> : ProcessManagerConfiguration<TState, TActor> where TState : class, IProcessState {
        public ClusterProcessConfiguration(IProcessDependencyFactory<TState> factory, string statePath) : base(factory, statePath)
        {

        }

        protected override Parameter[] CreateParametersRegistration()
        {
            return base.CreateParametersRegistration()
                       .Union(new[]
                              {
                                  new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IRecycleConfiguration),
                                                        (pi, ctx) => ProcessDependencyFactory.CreateRecycleConfiguration())
                              })
                       .ToArray();
        }
    }
}