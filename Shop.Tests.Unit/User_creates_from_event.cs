using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Events;

namespace Shop.Tests.Unit
{
    [TestFixture]
    public class User_creates_from_event : AggregateTest<User>
    {
        [Test]
        public void User_receives_id()
        {
            Assert.AreEqual(_createdEvent.Id, Aggregate.Id);
        }

        [Test]
        public void User_receives_login()
        {
            Assert.AreEqual(_createdEvent.Login, Aggregate.Login);
        }

        [Test]
        public void User_receives_account()
        {
            Assert.AreEqual(_createdEvent.Account, Aggregate.Account);
        }

        private readonly UserCreated _createdEvent = new Fixture().Create<UserCreated>();

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return _createdEvent;
        }
    }
}
