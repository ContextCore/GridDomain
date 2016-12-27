using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;

namespace Shop.Tests.Unit.AccountAggregate.Aggregate
{
    [TestFixture]
    public class Account_creation_tests : AggregateCommandsTest<Account,AccountCommandsHandler>
    {
        private CreateAccountCommand _command;

        [OneTimeSetUp]
        public void When_creating_account()
        {
            Init();
            _command = new CreateAccountCommand(Aggregate.Id,Guid.NewGuid(),123);
            Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new AccountCreated(_command.AccountId,
                                                 _command.UserId,
                                                 _command.Number);
        }

        [Test]
        public void Aggregate_should_take_id_from_command()
        {
            Assert.AreEqual(_command.AccountId, Aggregate.Id);
        }

        [Test]
        public void Aggregate_should_take_number_from_command()
        {
            Assert.AreEqual(_command.Number, Aggregate.Number);
        }

        [Test]
        public void Aggregate_should_take_user_from_command()
        {
            Assert.AreEqual(_command.UserId, Aggregate.UserId);
        }
    }
}