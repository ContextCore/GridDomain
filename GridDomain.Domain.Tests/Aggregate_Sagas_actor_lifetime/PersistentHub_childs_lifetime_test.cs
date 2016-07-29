using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class PersistentHub_childs_lifetime_test: ProgrammingSoftwareSagaTest
    {
        protected Guid _aggregateId => _infrastructure.ChildId;
        protected IPersistentActorTestsInfrastructure _infrastructure;
        private readonly PersistentHubTestsStatus.PersistenceCase _case;

        public PersistentHub_childs_lifetime_test(PersistentHubTestsStatus.PersistenceCase @case)
        {
            _case = @case;
        }

       
        protected void When_hub_creates_a_child()
        {
            _infrastructure.Hub.Tell(_infrastructure.ChildCreateMessage);

            Thread.Sleep(100);
        }

        protected void And_it_is_not_active_until_lifetime_period_is_expired()
        {
            Thread.Sleep(3000);
        }

        protected void And_command_for_child_is_sent()
        {
            _infrastructure.Hub.Tell(_infrastructure.ChildActivateMessage);
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
                                _infrastructure = new AggregatePersistedHub_Infrastructure(GridNode.System);
                    break;

                case PersistentHubTestsStatus.PersistenceCase.Saga:
                    _infrastructure = new SagaPersistedHub_Infrastructure(GridNode.System);
                    break;
            }
        }
    }
}