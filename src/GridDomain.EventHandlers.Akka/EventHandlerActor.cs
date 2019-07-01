using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace GridDomain.EventHandlers.Akka
{
    public static class EventHandlerActor
    {
        public class Start
        {
            private Start()
            {
            }

            public static readonly Start Instance = new Start();
        }

        public class Next
        {
            private Next()
            {
            }

            public static readonly Next Instance = new Next();
        }

        public class Done
        {
            private Done()
            {
            }

            public static readonly Done Instance = new Done();
        }
    }


    public abstract class EventHandlerActor<THandler> : ReceiveActor
    {
        protected EventHandlerActor()
        {
            var log = Context.GetLogger();
            var handler = Context.System.GetEventHandlersExtension().GetHandler<THandler>();
            Receive<EventHandlerActor.Start>(s =>
            {
                log.Info("Starting projection");
                Sender.Tell(EventHandlerActor.Next.Instance);
            });
            
            Receive<EventHandlerActor.Done>(s =>
            {
                log.Info("Stopping projection");
            });
            
            Receive<Sequenced>(m =>
            {
                Handle(handler, m).ContinueWith(t => EventHandlerActor.Next.Instance).PipeTo(Sender);
            });
            
            ReceiveAny(o => log.Warning("missing message: " + o.ToString()));
        }

        protected abstract Task Handle(THandler handler, Sequenced m);

        protected class UnknownEventReceivedException : Exception
        {
        }
        
    }
    public class EventHandlerActor<TMessage,THandler> : EventHandlerActor<THandler> where THandler:IEventHandler<TMessage>
    {
        protected override Task Handle(THandler handler, Sequenced m)
        {
            switch (m.Message)
            {
                case TMessage msg:
                    return handler.Handle(new[] {new Sequenced<TMessage>(msg,m.Sequence)});
                default:
                    throw new UnknownEventReceivedException();
            }
        }
    }
    
    public class EventHandlerActor<TMessageA,TMessageB,THandler> : EventHandlerActor<THandler> where THandler:IEventHandler<TMessageA>, IEventHandler<TMessageB>
    {
        protected override Task Handle(THandler handler, Sequenced m)
        {
            switch (m.Message)
            {
                case TMessageA msg:
                    return handler.Handle(new[] {new Sequenced<TMessageA>(msg, m.Sequence)});
                case TMessageB msg:
                    return handler.Handle(new[] {new Sequenced<TMessageB>(msg, m.Sequence)});
                default:
                    throw new UnknownEventReceivedException();
            }
        }
    }
}