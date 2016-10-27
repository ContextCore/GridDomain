using System;
using System.Diagnostics;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class ProgrammingSoftwareSagaTest : SampleDomainCommandExecutionTests
    {
        protected override IMessageRouteMap CreateMap()
        {
            return new SoftwareProgrammingSagaRoutes();
        }

        protected IPublisher Publisher { get; private set; }
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        [SetUp]
        public void InitPublisher()
        {
            Publisher = GridNode.Container.Resolve<IPublisher>();
        }
        protected override IContainerConfiguration CreateConfiguration()
        {
            var baseConf = base.CreateConfiguration();

            return new CustomContainerConfiguration(
                c => c.RegisterSaga<SoftwareProgrammingSaga,
                                    SoftwareProgrammingSagaData,
                                    SoftwareProgrammingSagaFactory,
                                    SleptWellEvent,
                                    GotTiredEvent>(SoftwareProgrammingSaga.Descriptor),

                c => c.Register(baseConf),
                c => c.RegisterAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>,
                    SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>()
                );
        }

        public ProgrammingSoftwareSagaTest(bool inMemory = true) : base(inMemory)
        {
        }
    }
}