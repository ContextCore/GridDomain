using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaRoutingActor : TypedActor, IHandler<CreateRoute>
    {
        private Logger _log = LogManager.GetCurrentClassLogger();
        private IActorRef _distributedTransport;
        //private Cluster _cluster;
        //#  private HashSet<CreateRoute>  
      
        protected override void PreStart()
        {
            _distributedTransport = DistributedPubSub.Get(Context.System).Mediator;
         //   _cluster = Akka.Cluster.Cluster.Get(Context.System);
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
            var actorType = typeof (MessageHandlingActor<,>).MakeGenericType(msg.MessageType, msg.HandlerType);
            var handleActorProps = Context.System.DI().Props(actorType);
           
         //   var router = new ConsistentHashingPool(Environment.ProcessorCount).WithHashMapping(o => );
          //  router.Props(handleActorProps);
            var handleActor = Context.System.ActorOf(handleActorProps);

          //  _cluster.
            return handleActor;
        }
    }
}