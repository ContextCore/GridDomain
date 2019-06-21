using System;
using GridDomain.EventHandlers;
using GridDomain.EventHandlers.Akka;

namespace GridDomain.Projections.Akka
{
   public class ProjectionActor<TMessage, THandler> : EventHandlerActor<TMessage, THandler>
      where THandler : IEventHandler<TMessage>
   {
      
   }
}