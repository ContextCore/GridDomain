using System;
using System.Linq;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public static class SagaExtensions
    {
        public static ISagaDescriptor GetDescriptor<TSaga,TSagaData>(this TSaga saga) 
            where TSagaData : class, ISagaState
            where TSaga: Saga<TSagaData>
        {
            var descriptor = new SagaDescriptor<ISagaInstance<TSaga, TSagaData>, SagaDataAggregate<TSagaData>>();

            FillAcceptMessages(saga, descriptor);
            FillCommands(saga, descriptor);
            FillStartMessages(saga, descriptor);

            return descriptor;
        }

        private static void FillStartMessages<TSaga, TSagaData>(TSaga saga, SagaDescriptor<ISagaInstance<TSaga, TSagaData>, SagaDataAggregate<TSagaData>> descriptor)
            where TSagaData : class, ISagaState where TSaga : Saga<TSagaData>
        {
            foreach (var startMessage in saga.StartMessages)
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
            foreach (var eventType in saga.Events.Select(e => e.GetType())
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
}