using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.Actors
{
   
    //Actor provide synchronous, in-order processing of different messages
    //Can be used for dependent projection builders

    class MessageHandlerInformation
    {
        public MessageHandlerInformation(Type handler, Type message)
        {
            Handler = handler;
            Message = message;
        }

        public Type Message { get; }
        public Type Handler { get; }
    }

    class InitHandlers
    {
        public MessageHandlerInformation[] MessageHandlers { get; }

        public InitHandlers(MessageHandlerInformation[] messageHandlers)
        {
            MessageHandlers = messageHandlers;
        }
    }

    class SynchronizationProcessingActor : UntypedActor
    {
        private readonly IServiceLocator _serviceLocator;
        private IDictionary<Type,IHandler<object>> messageHandlers = new Dictionary<Type, IHandler<object>>(); 

        public SynchronizationProcessingActor(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        protected override void OnReceive(object message)
        {
            var init  = message as InitHandlers;
            if (init != null)
            {
                messageHandlers = init.MessageHandlers.ToDictionary(c => c.Message, c => (IHandler<object>)_serviceLocator.Resolve(c.Handler));
                return;
            }

            var handler = messageHandlers[message.GetType()];
            handler.Handle(message);
        }
    }
}
