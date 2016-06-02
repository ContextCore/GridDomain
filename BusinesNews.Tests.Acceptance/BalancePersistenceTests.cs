using System;
using CommonDomain.Persistence;
using GridDomain.Balance.Domain.AccountAggregate;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.Persistence;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Acceptance.Balance
{
    [TestFixture]
    public class BalancePersistenceTests
    {
        [SetUp]
        public void ClearDb()
        {
            TestDbTools.ClearData(TestEnvironment.Configuration);
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
            var initialBalance = new Fixture().Create<Account>();

            var repo = CreateIndependentRepository();
            repo.Save(initialBalance, Guid.NewGuid());

            var repo1 = CreateIndependentRepository();
            var restoredBalance = repo1.GetById<Account>(initialBalance.Id);
            Assert.True(new CompareLogic().Compare(restoredBalance, initialBalance).AreEqual);
        }
    }
}