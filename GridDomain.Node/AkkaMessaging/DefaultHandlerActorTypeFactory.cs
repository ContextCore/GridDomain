using System;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging
{
    public class DefaultHandlerActorTypeFactory : IHandlerActorTypeFactory
    {
        public Type GetActorTypeFor(Type message, Type handler)
        {
            return typeof(MessageProcessActor<,>).MakeGenericType(message, handler);
        }
    }
}