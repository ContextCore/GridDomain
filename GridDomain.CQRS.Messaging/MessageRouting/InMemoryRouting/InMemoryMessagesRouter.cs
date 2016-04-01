using System;
using GridDomain.Logging;
using NLog;

namespace GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting
{
    public class InMemoryMessagesRouter : IMessagesRouter
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ISubscriber _subscriber;

        public InMemoryMessagesRouter(ISubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new RouteBuilder<TMessage>(this);
        }

        public void Register<TMessage, THandler>(Func<TMessage, THandler> handlerFactory)
            where THandler : IHandler<TMessage>
        {
        }

        private IHandler<TMessage> CreateHandler<TMessage, THandler>
            (Func<TMessage, THandler> commandHandlerFactory, TMessage msg) where THandler : IHandler<TMessage>
        {
            try
            {
                _log.Trace($"создаётся обработчик для сообщения типа {typeof (TMessage).Name}\r\n" +
                           $"тело сообщения\r\n:{msg.ToPropsString()}");
                return commandHandlerFactory(msg);
            }
            catch (Exception ex)
            {
                _log.Fatal(
                    $"При создании обработчика {typeof (THandler).Name} для сообщения типа {typeof (TMessage).Name} " +
                    $"возникла ошибка: \r\n" + ex);
                return null;
            }
        }

        private void ProcessMessage<TMessage, THandler>(Func<TMessage, THandler> commandHandlerFactory, TMessage msg)
            where THandler : IHandler<TMessage>
        {
            var handler = CreateHandler(commandHandlerFactory, msg);
            if (handler == null) return;

            InvokeHandler<TMessage, THandler>(msg, handler);
        }

        //TODO:добавить политику кэширования уже созданных обработчиков 
        private void InvokeHandler<TMessage, THandler>(TMessage msg, IHandler<TMessage> handler)
            where THandler : IHandler<TMessage>
        {
            var handlerName = handler.GetType().Name;
            var name = typeof (TMessage).Name;
            try
            {
                var propsString = msg.ToPropsString();

                _log.Trace($"обработчик {handlerName} будет вызван для сообщения  {name}\r\n" +
                           $"тело сообщения:\r\n {propsString}");

                handler.Handle(msg);

                _log.Trace($"обработчик {handlerName} закончил обработку сообщения  {name}\r\n" +
                           $"тело сообщения:\r\n {propsString}");
            }
            catch (Exception ex)
            {
                _log.Fatal($"При обработке сообщения  {name} обработчиком {handlerName}" +
                           " возникла ошибка: \r\n" + ex);
            }
        }
    }
}