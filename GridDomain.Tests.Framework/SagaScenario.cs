using System;
using System.Linq;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Framework
{
    public class SagaScenario<TSaga, TData, TFactory>
        where TSaga: Saga<TData>
        where TData : class, ISagaState
        where TFactory : class, ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>
    {
        protected ISagaProducer<ISagaInstance<TSaga,TData>> SagaProducer { get; }
        public ISagaInstance<TSaga, TData> SagaInstance { get; private set; }
        protected SagaDataAggregate<TData> SagaStateAggregate { get; private set; }

        protected ICommand[] ExpectedCommands { get; private set; } = { };
        protected ICommand[] ProducedCommands { get; private set; } = { };
        protected DomainEvent[] GivenEvents { get; private set; } = { };
        protected DomainEvent[] ReceivedEvents { get; private set; } = { };

        internal SagaScenario(ISagaProducer<ISagaInstance<TSaga, TData>> producer)
        {
            SagaProducer = producer;
        }

        public static SagaScenario<TSaga, TData, TFactory> New(ISagaDescriptor descriptor,
                                                               TFactory factory = null)
        {
            dynamic dynamicfactory = factory ?? CreateSagaFactory();
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);

            foreach(var startMessageType in descriptor.StartMessages)
                    producer.Register(startMessageType,
                        msg =>
                        {
                            var saga = dynamicfactory.Create((dynamic)msg);
                            return (ISagaInstance<TSaga, TData>) saga;
                        });

            return new SagaScenario<TSaga, TData, TFactory>(producer);
        }

        public SagaScenario<TSaga, TData, TFactory> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            SagaStateAggregate = CreateAggregate<SagaDataAggregate<TData>>();
            SagaStateAggregate.ApplyEvents(events);
            SagaStateAggregate.ClearEvents();
            return this;
        }
     
        public SagaScenario<TSaga, TData, TFactory> When(params DomainEvent[] events)
        {
            ReceivedEvents = events;
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> Then(params Command[] expectedCommands)
        {
            ExpectedCommands = expectedCommands;
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> Run()
        {

           if (SagaStateAggregate != null)
                SagaInstance = SagaProducer.Create(SagaStateAggregate);

           foreach (var evt in ReceivedEvents.Where(e => SagaProducer.KnownDataTypes.Contains(e.GetType())))
                SagaInstance = SagaProducer.Create(evt);

           //When

           foreach(var evt in ReceivedEvents)
                //cast to allow dynamic to locate Transit method
                (SagaInstance as ISagaInstance).Transit((dynamic)evt);

            //Then
           ProducedCommands = SagaInstance.CommandsToDispatch.ToArray();

           return this;
       }

       public void Check()
       {
           EventsExtensions.CompareCommands(ExpectedCommands, ProducedCommands);
       }

        private static TFactory CreateSagaFactory()
        {
            var constructorInfo = typeof(TFactory).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (TFactory)constructorInfo.Invoke(null);
        }
        private static T CreateAggregate<T>()
        {
            return (T)(new AggregateFactory().Build(typeof(T), Guid.NewGuid(), null));
        }

    }
}