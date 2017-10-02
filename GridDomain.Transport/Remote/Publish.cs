using System;

namespace GridDomain.Transport.Remote
{
    public class Publish
    {
        public Publish(object msg, Type topic = null)
        {
            Msg = msg;
            Topic = topic ?? msg.GetType();
        }

        public object Msg { get; }
        public Type Topic { get; }
    }
}