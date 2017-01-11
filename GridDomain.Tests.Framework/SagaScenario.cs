using System;
using System.Linq;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;

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

        public TData GenerateState(string stateName,Func<ICustomizationComposer<TData>,IPostprocessComposer<TData>> fixtureConfig = null)
        {
            var fixture = new Fixture();
            var composer = fixtureConfig?.Invoke(fixture.Build<TData>());
            var generateState = composer != null ? composer.Create() : fixture.Create<TData>();
            generateState.CurrentStateName = stateName;
            return generateState;
        }

        public static SagaScenario<TSaga, TData, TFactory> New(ISagaDescriptor descriptor,
                                                               TFactory factory = null)
        {
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);
            producer.RegisterAll<TFactory,TData>(factory ?? CreateSagaFactory());
            return new SagaScenario<TSaga, TData, TFactory>(producer);
        }

        public SagaScenario<TSaga, TData, TFactory> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            SagaStateAggregate = CreateAggregate<SagaDataAggregate<TData>>(Guid.NewGuid());
            SagaStateAggregate.ApplyEvents(events);
            SagaStateAggregate.ClearEvents();
            return this;
        }
        public SagaScenario<TSaga, TData, TFactory> GivenState(Guid id, TData state)
        {
            InitialState = state;
            SagaStateAggregate = CreateAggregate<SagaDataAggregate<TData>>(id);
            SagaStateAggregate.ApplyEvents(new SagaCreatedEvent<TData>(state,id));
            SagaStateAggregate.ClearEvents();
            return this;
        }

        public TData InitialState { get; private set; }

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

        public SagaScenario<TSaga, TData, TFactory> CheckProducedCommands()
        {
            EventsExtensions.CompareCommands(ExpectedCommands, ProducedCommands);
            return this;

        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedState(TData expectedState, CompareLogic customCompareLogic = null)
        {
            EventsExtensions.CompareState(expectedState, SagaInstance.Data.Data, customCompareLogic);
            return this;
        }
      

        public SagaScenario<TSaga, TData, TFactory> CheckProducedStateIsNotChanged()
        {
            EventsExtensions.CompareState(InitialState, SagaInstance.Data.Data);
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedStateName(string stateName)
        {
            Assert.AreEqual(stateName, SagaInstance.Data.Data.CurrentStateName);
            return this;
        }
        public SagaScenario<TSaga, TData, TFactory> CheckOnlyStateNameChanged(string stateName)
        {
            CheckProducedStateName(stateName);
            EventsExtensions.CompareStateWithoutName(InitialState, SagaInstance.Data.Data);

            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedStateName(Func<TData> expectedStateProducer)
        {
            return CheckProducedState(expectedStateProducer());
        }

        private static TFactory CreateSagaFactory()
        {
            var constructorInfo = typeof(TFactory).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (TFactory)constructorInfo.Invoke(null);
        }
        private static T CreateAggregate<T>(Guid id)
        {
            return (T)(new AggregateFactory().Build(typeof(T), id, null));
        }

    }
}