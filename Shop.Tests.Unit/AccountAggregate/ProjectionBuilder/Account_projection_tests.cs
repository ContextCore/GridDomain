using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.AccountAggregate.ProjectionBuilder
{
    [TestFixture]
    public class Account_projection_tests : Account_projection_builder_test
    {
        private AccountCreated _msg;

        [OneTimeSetUp]
        public void Given_account_created_and_projecting()
        {
            _msg = new AccountCreated(Guid.NewGuid(), Guid.NewGuid(), 42);
            var user = new User {Id = _msg.UserId, Login = "test"};

            using (var context = ContextFactory())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            ProjectionBuilder.Handle(_msg);
        }

        [Test]
        public void Shoud_create_account_row()
        {
            using (var context = ContextFactory())
            {
                var row = context.Accounts.Find(_msg.SourceId);
                Assert.NotNull(row);
            }
        }

        [Test]
        public void Should_fail_on_additional_project_attempt()
        {
            Assert.Throws<ArgumentException>(() => ProjectionBuilder.Handle(_msg));
        }

        [Test]
        public void Should_project_all_fields()
        {
            using (var context = ContextFactory())
            {
                var row = context.Accounts.Find(_msg.SourceId);
                var user = context.Users.Find(_msg.UserId);

                Assert.AreEqual(row.UserId, _msg.UserId);
                Assert.AreEqual(row.Number, _msg.AccountNumber);
                Assert.AreEqual(row.Created, _msg.CreatedTime);
                Assert.AreEqual(row.Id, _msg.SourceId);
                Assert.AreEqual(row.Login, user.Login);
            }
        }
    }
}