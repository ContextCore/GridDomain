using System;
using System.Linq;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public static class SagaExtensions
    {
        public static ISagaDescriptor GetDescriptor<TSagaData>(this Saga<TSagaData> saga) where TSagaData : class, ISagaState
        {
            var descriptor = new SagaDescriptor();
            FillAcceptMessages(saga, descriptor);
            FillCommands(saga, descriptor);

            descriptor.SagaType = typeof(ISagaInstance<,>).MakeGenericType(saga.GetType(), typeof(TSagaData));
            descriptor.StateType =  typeof (SagaDataAggregate<TSagaData>);
            descriptor.StartMessage = saga.StartMessage;

            return descriptor;
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