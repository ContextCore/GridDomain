using System;
using System.Threading;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Domain.AccountAggregate.Events;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.OfferAggregate;
using BusinessNews.Domain.Sagas.BuySubscription;
using BusinessNews.Node;
using BusinessNews.ReadModel;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Configuration;
using Microsoft.Practices.Unity;
using NMoneys;
using NUnit.Framework;

namespace BusinesNews.Tests.Acceptance.BuySubscriptionSaga
{
    [TestFixture]
    class Given_business_with_low_balance : NodeCommandsTest
    {
        private readonly Guid _accountId = Guid.NewGuid();
        private readonly Guid _businessId = Guid.NewGuid();
        private readonly Guid _subscriptionId = Guid.NewGuid();
        private readonly Money _amount = new Money(1);
        private object _sagaResultMessage;

        public Given_business_with_low_balance() : base(new AutoTestAkkaConfiguration().ToStandAloneSystemConfig())
        {
        }

        [TestFixtureSetUp]
        public void Given_business_with_money()
        {
            var registerNewBusinessCommand = new RegisterNewBusinessCommand(_businessId, "test business", _accountId);
            ExecuteAndWaitFor<BusinessCreatedEvent>(registerNewBusinessCommand);

            var createAccountCommand = new CreateAccountCommand(_accountId, _businessId);
            ExecuteAndWaitFor<BusinessBalanceCreatedProjectedNotification>(createAccountCommand);

            var replenishAccountByCardCommand = new ReplenishAccountByCardCommand(_accountId, _amount, null);
            ExecuteAndWaitFor<AccountBalanceReplenishEvent>(replenishAccountByCardCommand);

            var orderSubscriptionCommand = new OrderSubscriptionCommand(_businessId, VIPSubscription.ID, _subscriptionId);

            var expectedMsg = 
            ExecuteAndWaitFor<SubscriptionOrderCompletedEvent,
                              SagaFault<BuySubscriptionSagaStateAggregate>>(orderSubscriptionCommand);

            _sagaResultMessage = expectedMsg.Message;
            Thread.Sleep(2000); //to build up read model
        }

        [Test]
        public void Saga_result_is_saga_fault()
        {
            Assert.IsInstanceOf<SagaFault<BuySubscriptionSagaStateAggregate>>(_sagaResultMessage);
        }

        [Test]
        public void Saga_fault_contains_command_fault_for_pay_for_bill_command()
        {
            var fault = (SagaFault<BuySubscriptionSagaStateAggregate>)(_sagaResultMessage);
            Assert.IsInstanceOf<CommandFault<PayForBillCommand>>(fault.CommandFault);
        }

        [Test]
        public void Command_fault_contains_exception()
        {
            var fault = (SagaFault<BuySubscriptionSagaStateAggregate>) (_sagaResultMessage);
            var commandFault = (CommandFault<PayForBillCommand>) fault.CommandFault;
            Assert.IsNotNull(commandFault.Exception);
        }


        [Test]
        public void BusinessBalance_in_read_model_should_remains_the_same()
        {
            var offer = WellKnownOffers.Catalog[VIPSubscription.ID];
            using (var context = new BusinessBalanceContext(new LocalDbConfiguration().ReadModelConnectionString))
            {
                var businessBalance = context.Accounts.Find(_accountId);
                Assert.AreEqual(_amount.Amount, businessBalance.Amount);
            }
        }

        [Test]
        public void BusinessSubscription_in_write_model_should_not_be_set()
        {
            var business = LoadAggregate<Business>(_businessId);
            Assert.AreNotEqual(_subscriptionId, business.SubscriptionId);
        }

        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(20);
        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container = new UnityContainer();
            BusinessNews.Node.CompositionRoot.Init(container, new LocalDbConfiguration());
            return new GridDomainNode(container, new BusinessNewsRouting(), TransportMode.Standalone, Sys);
        }
    }
}