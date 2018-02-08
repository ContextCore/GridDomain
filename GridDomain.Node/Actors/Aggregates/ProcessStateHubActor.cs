using System;
using GridDomain.Configuration;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Actors.Aggregates {
    public class ProcessStateHubActor<TState> : AggregateHubActor<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateHubActor(IRecycleConfiguration conf) : base(conf)
        {
            ChildActorType = typeof(ProcessStateActor<TState>);
            Receive<GetProcessState>(s =>
                                     {
                                         SendToChild(s, GetChildActorName(s.Id), Sender);
                                     });
        }
        protected override Type ChildActorType { get; }

        
    }
}