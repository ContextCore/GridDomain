using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.ProjectionBuilder
{
    public class Account_projection_tests : Account_projection_builder_test
    {
        private readonly AccountCreated _msg;

        public Account_projection_tests()
        {
            _msg = new AccountCreated(Guid.NewGuid(), Guid.NewGuid(), 42);
            var user = new User {Id = _msg.UserId, Login = "test"};

            using (var context = ContextFactory())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            ProjectionBuilder.Handle(_msg).Wait();
        }

        [Fact]
        public void Shoud_create_account_row()
        {
            using (var context = ContextFactory())
            {
                var row = context.Accounts.Find(_msg.SourceId);
                Assert.NotNull(row);
            }
        }

        [Fact]
        public async Task Should_fail_on_additional_project_attempt()
        {
           await Assert.ThrowsAsync<ArgumentException>(() => ProjectionBuilder.Handle(_msg));
        }

        [Fact]
        public void Should_project_all_fields()
        {
            using (var context = ContextFactory())
            {
                var row = context.Accounts.Find(_msg.SourceId);
                var user = context.Users.Find(_msg.UserId);

                Assert.Equal(row.UserId, _msg.UserId);
                Assert.Equal(row.Number, _msg.AccountNumber);
                Assert.Equal(row.Created, _msg.CreatedTime);
                Assert.Equal(row.Id, _msg.SourceId);
                Assert.Equal(row.Login, user.Login);
            }
        }
    }
}