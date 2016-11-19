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

    public class PublishMany
    {
        public Publish[] Messages;

        public PublishMany(params Publish[] messages)
        {
            Messages = messages;
        }
    }

    public class PublishManyAck
    {
        public static PublishManyAck Instance = new PublishManyAck();
        private PublishManyAck(){}
    }

    public class PublishAck
    {
        public static PublishAck Instance = new PublishAck();
        private PublishAck(){}
    }
}