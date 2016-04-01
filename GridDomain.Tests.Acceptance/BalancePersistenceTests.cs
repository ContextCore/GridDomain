using System;
using CommonDomain.Persistence;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.Persistence;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Acceptance
{
    [TestFixture]
    public class BalancePersistenceTests
    {
        [SetUp]
        public void ClearDb()
        {
            // IDbConfiguration autoTestMarteauConfiguration = TestEnvironment.Configuration;
            // TestDbTools.RecreateWriteDb(autoTestMarteauConfiguration.EventStoreConnectionString);

            // var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(TestEnvironment.Configuration);
            //     _workerGridDomainNode = new GridDomainNode(new AkkaConfiguration("Worker", 8081),
            //                                                     autoTestGridDomainConfiguration,
            //                                                      new UnityContainer());
        }

        private IRepository CreateIndependentRepository()
        {
            var container = new UnityContainer();
            var conf = TestEnvironment.Configuration;
            CompositionRoot.RegisterEventStore(container, conf);
            return container.Resolve<IRepository>();
        }

        [Test]
        public void Balance_should_persist_with_all_events()
        {
            var initialBalance = new Fixture().Create<Balance.Domain.Balance>();

            var repo = CreateIndependentRepository();
            repo.Save(initialBalance, Guid.NewGuid());

            var repo1 = CreateIndependentRepository();
            var restoredBalance = repo1.GetById<Balance.Domain.Balance>(initialBalance.Id);
            Assert.True(new CompareLogic().Compare(restoredBalance, initialBalance).AreEqual);
        }
    }
}