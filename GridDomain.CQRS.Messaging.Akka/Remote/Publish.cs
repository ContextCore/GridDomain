using System;

namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class Publish
    {
        public object Msg { get;}
        public Type Topic { get; }

        public Publish(object msg, Type topic )
        {
            Msg = msg;
            Topic = topic;
        }
    }

    public class PublishAck
    {
        public static PublishAck Instance = new PublishAck();
        private PublishAck(){}
    }
}