using System.Threading;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class PersistentHub_children_lifetime_test: ProgrammingSoftwareSagaTest
    {
        protected IPersistentActorTestsInfrastructure Infrastructure;
        private readonly PersistentHubTestsStatus.PersistenceCase _case;

        public PersistentHub_children_lifetime_test(PersistentHubTestsStatus.PersistenceCase @case)
        {
            _case = @case;
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return  new CustomContainerConfiguration(c => c.Register(base.CreateConfiguration()),
              c => c.RegisterStateSaga<GridDomain.Tests.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga,
                                       SoftwareProgrammingSagaState,
                                       GotTiredEvent,
                                       GridDomain.Tests.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSagaFactory>());
        }

        protected void When_hub_creates_a_child()
        {
            Infrastructure.Hub.Tell(Infrastructure.ChildCreateMessage);

            Thread.Sleep(200);
        }

        protected void And_it_is_not_active_until_lifetime_period_is_expired()
        {
            Thread.Sleep(3000);
        }

        protected void And_command_for_child_is_sent()
        {
            Infrastructure.Hub.Tell(Infrastructure.ChildActivateMessage);
            Thread.Sleep(100);
        }

        [SetUp]
        public void Clear_child_lifetimes()
        {
            PersistentHubTestsStatus.ChildTerminationTimes.Clear();
            PersistentHubTestsStatus.ChildExistence.Clear();

            switch (_case)
            {
                    case PersistentHubTestsStatus.PersistenceCase.Aggregate:
                                Infrastructure = new AggregatePersistedHub_Infrastructure(GridNode.System);
                    break;

                case PersistentHubTestsStatus.PersistenceCase.IstanceSaga:
                    Infrastructure = new InstanceSagaPersistedHub_Infrastructure(GridNode.System);
                    break;

                case PersistentHubTestsStatus.PersistenceCase.StateSaga:
                    Infrastructure = new StateSagaPersistedHub_Infrastructure(GridNode.System);
                    break;
                default: 
                    throw new UnknownCaseException();
            }
        }
    }
}