using System;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.AccountAggregate.ProjectionBuilder
{
    [TestFixture]
    public class Account_transactions_projection_tests : Account_projection_builder_test
    {
        private AccountCreated _msgCreated;
        private AccountReplenish _msgReplenish;
        private AccountWithdrawal _msgWithdrawal;

        [OneTimeSetUp]
        public void Given_account_created_and_projecting()
        {
            _msgCreated = new AccountCreated(Guid.NewGuid(), Guid.NewGuid(), 42);
            var user = new User {Id = _msgCreated.UserId, Login = "test"};

            _msgReplenish = new AccountReplenish(_msgCreated.SourceId, Guid.NewGuid(), new Money(100));
            _msgWithdrawal = new AccountWithdrawal(_msgCreated.SourceId, Guid.NewGuid(), new Money(30));

            using (var context = ContextFactory())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            ProjectionBuilder.Handle(_msgCreated);
            ProjectionBuilder.Handle(_msgReplenish);
            ProjectionBuilder.Handle(_msgWithdrawal);
        }

        [Test]
        public void Should_create_entries_for_replenish()
        {
            using (var context = ContextFactory())
            {
                var history = context.TransactionHistory.Find(_msgReplenish.ChangeId);
                Assert.NotNull(history);
            }
        }

        [Test]
        public void Should_create_entries_for_withdrawal()
        {
            using (var context = ContextFactory())
            {
                var history = context.TransactionHistory.Find(_msgWithdrawal.ChangeId);
                Assert.NotNull(history);
            }
        }

        [Test]
        public void Should_fill_fields_for_replenish()
        {
            using (var context = ContextFactory())
            {
                var history = context.TransactionHistory.Find(_msgReplenish.ChangeId);
                Assert.AreEqual(_msgReplenish.SourceId, history.AccountId);
                Assert.AreEqual(_msgReplenish.Amount.Amount, history.ChangeAmount);
                Assert.AreEqual(_msgReplenish.CreatedTime, history.Created);
                Assert.AreEqual(_msgReplenish.Amount.CurrencyCode.ToString(), history.Currency);
                Assert.AreEqual(0, history.InitialAmount);
                Assert.AreEqual(_msgReplenish.Amount.Amount, history.NewAmount);
                Assert.AreEqual(AccountOperations.Replenish, history.Operation);
                Assert.AreEqual(_msgReplenish.ChangeId, history.TransactionId);
                Assert.AreEqual(1, history.TransactionNumber);
            }
        }

        [Test]
        public void Should_fill_fields_for_withdrawal()
        {
            using (var context = ContextFactory())
            {
                var history = context.TransactionHistory.Find(_msgWithdrawal.ChangeId);

                Assert.AreEqual(_msgWithdrawal.SourceId, history.AccountId);
                Assert.AreEqual(_msgWithdrawal.Amount.Amount, history.ChangeAmount);
                Assert.AreEqual(_msgWithdrawal.CreatedTime, history.Created);
                Assert.AreEqual(_msgWithdrawal.Amount.CurrencyCode.ToString(), history.Currency);
                Assert.AreEqual(_msgReplenish.Amount.Amount, history.InitialAmount);
                Assert.AreEqual((_msgReplenish.Amount - _msgWithdrawal.Amount).Amount, history.NewAmount);
                Assert.AreEqual(AccountOperations.Withdrawal, history.Operation);
                Assert.AreEqual(_msgWithdrawal.ChangeId, history.TransactionId);
                Assert.AreEqual(2, history.TransactionNumber);
            }
        }

        [Test]
        public void Should_update_modified_time_for_account()
        {
            using (var context = ContextFactory())
            {
                var account = context.Accounts.Find(_msgWithdrawal.SourceId);
                Assert.AreEqual(_msgWithdrawal.CreatedTime, account.LastModified);
            }
        }
    }
}