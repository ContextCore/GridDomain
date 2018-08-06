using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Actor.Dsl;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.Hadlers;

namespace GridDomain.Node.Cluster.CommandPipe {



    public class ClusterHandlersPipeActorCell : UntypedActor
    {
        private IActorRef _handler;

        public ClusterHandlersPipeActorCell()
        {
            var props = Context.System.DI()
                               .Props<ClusterHandlersPipeActor>();

            _handler = Context.ActorOf(props,Self.Path.Name);
        }

        protected override void OnReceive(object message)
        {
            _handler.Forward(message);
        }
    }

    public class ClusterHandlersPipeActor : HandlersPipeActor
    {
        public ClusterHandlersPipeActor(MessageMap map, IActorRefAdapter processActor) : base(CreateRoutess(Context, map), processActor.GetRef()) { }

        private static IMessageProcessor CreateRoutess(IUntypedActorContext system, MessageMap messageRouteMap)
        {
            var catalog = new HandlersDefaultProcessor();
            foreach (var reg in messageRouteMap.Registratios)
            {
                var handlerActorType = typeof(MessageHandleActor<,>).MakeGenericType(reg.Message, reg.Handler);

                var props = system.DI()
                                  .Props(handlerActorType);

                var actor = system.ActorOf(props, handlerActorType.BeautyName());

                IMessageProcessor processor;
                switch (reg.ProcesType)
                {
                    case MessageMap.HandlerProcessType.Sync:
                        processor = new ActorAskMessageProcessor<HandlerExecuted>(actor);
                        break;

                    case MessageMap.HandlerProcessType.FireAndForget:
                        processor = new FireAndForgetActorMessageProcessor(actor);
                        break;
                    default:
                        throw new NotSupportedException(reg.ProcesType.ToString());
                }

                catalog.Add(reg.Message, processor);
            }

            return catalog;
        }
    }
}