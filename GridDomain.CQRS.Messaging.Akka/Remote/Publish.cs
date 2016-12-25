using System;

namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class Publish
    {
        public object Msg { get;}
        public Type Topic { get; }

        public Publish(object msg, Type topic = null)
        {
            Msg = msg;
            Topic = topic ?? msg.GetType();
        }
    }
}