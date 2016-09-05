using System;
using System.Linq;
using System.Linq.Expressions;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public static class SagaExtensions
    {
        public static ISagaDescriptor CreateDescriptor<TSaga, TSagaData, TStartMessagaA>(Expression<Func<TStartMessagaA,string>> correlationField = null )
        where TSagaData : class, ISagaState
        where TSaga : Saga<TSagaData>, new()
        {
            return CreateDescriptor<TSaga, TSagaData>(new TSaga(), typeof(TStartMessagaA));
        }

        public static ISagaDescriptor CreateDescriptor<TSaga, TSagaData, TStartMessagaA,TStartMessagaB>()
         where TSagaData : class, ISagaState
         where TSaga : Saga<TSagaData>, new()
        {
            return CreateDescriptor<TSaga, TSagaData>(new TSaga(), typeof(TStartMessagaA), typeof(TStartMessagaB));
        }

        public static ISagaDescriptor CreateDescriptor<TSaga, TSagaData, TStartMessagaA, TStartMessagaB, TStartMessagaC>()
         where TSagaData : class, ISagaState
         where TSaga : Saga<TSagaData>, new()
        {
            return CreateDescriptor<TSaga, TSagaData>(new TSaga(),
                                                      typeof(TStartMessagaA),
                                                      typeof(TStartMessagaB),
                                                      typeof(TStartMessagaC));
        }

        public static ISagaDescriptor CreateDescriptor<TSaga, TSagaData>(params Type[] startMessages)
            where TSagaData : class, ISagaState
            where TSaga : Saga<TSagaData>, new()
        {
            return CreateDescriptor<TSaga, TSagaData>(new TSaga(),startMessages);
        }

        public static ISagaDescriptor CreateDescriptor<TSaga,TSagaData>(this TSaga saga, params Type[] startMessages) 
            where TSagaData : class, ISagaState
            where TSaga: Saga<TSagaData>
        {
            if (!startMessages.Any())
                throw new StartMessagesMissedException();

            var descriptor = new SagaDescriptor(typeof(ISagaInstance<TSaga, TSagaData>),typeof(SagaDataAggregate<TSagaData>));

            FillAcceptMessages(saga, descriptor);
            FillCommands(saga, descriptor);
            FillStartMessages(startMessages, descriptor);

            return descriptor;
        }

        private static void FillStartMessages(Type[] startMessages, SagaDescriptor descriptor)
        {
            foreach (var startMessage in startMessages)
                descriptor.AddStartMessage(startMessage);
        }

        private static void FillCommands<T>(Saga<T> saga, SagaDescriptor descriptor) where T : class, ISagaState
        {
            foreach (var cmdType in saga.DispatchedCommands)
            {
                descriptor.AddProduceCommandMessage(cmdType);
            }
        }

        private static void FillAcceptMessages<T>(Saga<T> saga, SagaDescriptor descriptor) where T : class, ISagaState
        {
            foreach (var eventType in saga.Events
                                          .Select(e => e.GetType())
                                          .Where(t => t.IsGenericType))
            {
                var genericArguments = eventType.GetGenericArguments();
                var domainEventType = genericArguments.First();
                if (genericArguments.Length > 1)
                    throw new InvalidOperationException(
                        "Expected Event<> type to extract information, but found more then one generic argument");

                descriptor.AddAcceptedMessage(domainEventType);
            }
        }
    }

    public class StartMessagesMissedException : Exception
    {
        public StartMessagesMissedException():base("Saga descriptor should contains at least one start message")
        {
            
        }
    }
}