using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit
{
    public class Given_new_user
    {
        [Fact]
        public void It_should_emit_user_created_event()
        {
            var user =new User(Guid.NewGuid(),"test_login",Guid.NewGuid());
            var e = user.GetEvent<UserCreated>();
            user.PersistAll();
            Assert.Equal(e.Account, user.Account);
            Assert.Equal(e.Id, user.Id);
            Assert.Equal(e.Login, user.Login);
        }
    }
}