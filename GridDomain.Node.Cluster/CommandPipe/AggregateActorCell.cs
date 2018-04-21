using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class AggregateActorCell<TAggregate>: ReceiveActor where TAggregate : class, IAggregate
    {
        public AggregateActorCell()
        {
            var props = Context.System.DI()
                               .Props<ClusterAggregateActor<TAggregate>>();
                               
            var aggregate = Context.ActorOf(props, Self.Path.Name);
            
            ReceiveAny(o => aggregate.Forward(o));
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(ex =>
                                         {
                                             switch (ex)
                                             {
                                                 case CommandExecutionFailedException cf:
                                                     return Directive.Restart;
                                                 case CommandAlreadyExecutedException cae:
                                                     return Directive.Restart;
                                                 default:
                                                     return Directive.Stop;
                                             }
                                         });
        }
    }
    
    
     public class ProcessActorCell<TState>: ReceiveActor where TState : class, IProcessState
    {
        public ProcessActorCell()
        {
            var props = Context.System.DI()
                               .Props<ProcessActor<TState>>();
                               
            var process = Context.ActorOf(props, Self.Path.Name);
            
            ReceiveAny(o => process.Forward(o));
        }
        
      //  protected override SupervisorStrategy SupervisorStrategy()
      //  {
      //      return new OneForOneStrategy(ex =>
      //                                   {
      //                                       switch (ex)
      //                                       {
      //                                           case CommandExecutionFailedException cf:
      //                                               return Directive.Restart;
      //                                           case CommandAlreadyExecutedException cae:
      //                                               return Directive.Restart;
      //                                           default:
      //                                               return Directive.Stop;
      //                                       }
      //                                   });
      //  }
    }
    
    
    
}