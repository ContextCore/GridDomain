using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Autofac;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Common;

namespace GridDomain.EventHandlers.Akka {

    
    public class EventHandlersExtension : IExtension
    {
        private readonly ContainerBuilder _containerBuilder;
        private IContainer _container;
        private readonly ActorSystem _system;
        private readonly IList<StreamActorStarter> _eventStreamActors = new List<StreamActorStarter>();

        class StreamActorStarter
        {
            private readonly IActorRef _actor;
            private readonly object _startMessage;
            private readonly Predicate<object> _ackChecker;

            public StreamActorStarter(IActorRef actor, object startMessage, Predicate<object> ackChecker)
            {
                _ackChecker = ackChecker;
                _startMessage = startMessage;
                _actor = actor;
            }

            public async Task Start()
            {
                var ack = await _actor.Ask(_startMessage,TimeSpan.FromSeconds(5));

                if (!_ackChecker(ack))
                    throw new InvalidStartAcknowledgementReceived();
            }
            
            internal class InvalidStartAcknowledgementReceived : Exception
            {
            }
        }
        
        public EventHandlersExtension(ActorSystem system, ContainerBuilder container=null)
        {
            _system = system;
            _containerBuilder = container ?? new ContainerBuilder();
        }

        public void FinishRegistration()
        {
            _container = _containerBuilder.Build();
        }
        public T GetHandler<T>()
        {
            _container.TryResolve(typeof(T), out var handler);
            return (T)handler;
        }

        public Source<EventEnvelope, NotUsed> GetSource(string sourceName)
        {
            return _container.ResolveNamed<Source<EventEnvelope, NotUsed>>(sourceName);
        }
        public void RegisterSource(string sourceName,Source<EventEnvelope, NotUsed> source)
        {
            _containerBuilder.RegisterInstance(source).Named<Source<EventEnvelope, NotUsed>>(sourceName);
        }

        public Task Start()
        {
            return Task.WhenAll(_eventStreamActors.Select(s => s.Start())).TimeoutAfter(TimeSpan.FromSeconds(5));
        }
        public void RegisterEventHandler<TEvent, THandler>(string name=null, string nodeRole=null) where THandler:IEventHandler<TEvent> where TEvent : class
        {
            var streamName = name ?? typeof(THandler).GetStreamName<TEvent>();
            var actorName = "Stream_" + streamName;
            var props = Props.Create<SingleHandlerEventStreamActor<TEvent, THandler>>(streamName);
            
            var dispatcher = _system.ActorOf(ClusterSingletonManager.Props(
                    props,
                    PoisonPill.Instance,
                    ClusterSingletonManagerSettings.Create(_system).WithRole(nodeRole)
                ),
                name: actorName);
      
            var streamActor = _system.ActorOf(ClusterSingletonProxy.Props(
                    singletonManagerPath: "/user/"+actorName,
                    settings: ClusterSingletonProxySettings.Create(_system).WithRole(nodeRole)),
                name: actorName + "Proxy");
            
            _eventStreamActors.Add(new StreamActorStarter(streamActor,EventStreamActor.Start.Instance, o => o is EventStreamActor.Started));
        }
        
        public void RegisterEventHandler<TEventA, TEventB, THandler>() where THandler:IEventHandler<TEventA>,IEventHandler<TEventB>
        {
            throw new NotImplementedException();
        }
        
        public void RegisterEventHandler<TEventA, TEventB, TEventC, THandler>()where THandler:IEventHandler<TEventA>,IEventHandler<TEventB>,IEventHandler<TEventC>
        {
            throw new NotImplementedException();

        }
        
        public void RegisterEventHandler<TEventA, TEventB, TEventC, TEventD, THandler>()where THandler:IEventHandler<TEventA>,IEventHandler<TEventB>,IEventHandler<TEventC>,IEventHandler<TEventD>
        {
            throw new NotImplementedException();
        }
    }

 
}