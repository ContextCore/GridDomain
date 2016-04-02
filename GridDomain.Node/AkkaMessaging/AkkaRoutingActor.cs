using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.DI.Core;
using Akka.Routing;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaRoutingActor : TypedActor, IHandler<CreateRoute>
    {
        private Logger _log = LogManager.GetCurrentClassLogger();
        private IActorRef _distributedTransport;
        //#  private HashSet<CreateRoute>  
      
        protected override void PreStart()
        {
            _distributedTransport = DistributedPubSub.Get(Context.System).Mediator;
        }

        public void Handle(SubscribeAck msg)
        {
            _log.Trace($"Subscription was successfull for {msg.ToPropsString()}");
        }

        public void Handle(CreateRoute msg)
        {
            _log.Trace($"Routing actor creating route: {msg.ToPropsString()}");
            var handleActor = GetWorkerActorRef(msg);
            _log.Trace($"Created message handling actor for {msg.ToPropsString()}");
            var topic = msg.MessageType.FullName;
            _log.Trace($"Subscribing handler actor {handleActor.Path} to topic {topic}");
           var r = _distributedTransport.Ask(new Subscribe(topic, handleActor)).Result;
            _log.Trace($"Subscribed handler actor {handleActor.Path} to topic {topic}");
        }

        private static IActorRef GetWorkerActorRef(CreateRoute msg)
        {
            var router = new ConsistentHashingPool(Environment.ProcessorCount)
                                                  .WithHashMapping(m =>
                                                  {
                                                      var msgType = m.GetType();
                                                      if (msgType == msg.MessageType)
                                                      {
                                                          return msgType.GetProperty(msg.MessageCorrelationProperty)
                                                                        .GetValue(m);
                                                      }
                                                      return null;
                                                  });

            var actorType = typeof(MessageHandlingActor<,>).MakeGenericType(msg.MessageType, msg.HandlerType);
            var handleActorProps = Context.System.DI().Props(actorType).WithRouter(router);
            var handleActor = Context.System.ActorOf(handleActorProps);

            return handleActor;
        }
    }
}