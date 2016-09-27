using System;
using System.Threading;
using GridDomain.Common;
using GridDomain.Tests.Framework;
using NUnit.Framework;

namespace GridDomain.Tests
{
    [TestFixture]
    public class DateTimeSpec
    {
        [Test]
        public void When_EditableDateTimeStrategy_is_used_and_Now_is_set_Then_DateTimeFacadeNow_should_be_the_same()
        {
            var testTime = DateTime.Now.AddSeconds(-3);
            DateTimeStrategyHolder.Current = new TestDateTimeStrategy
            {
                EditableNow = testTime,
            };
            var now = BusinessDateTime.Now;
            Assert.True(now == testTime);
        }

        [Test]
        public void When_DefaultDateTimeStrategy_is_used_Then_DateTimeNow_should_differ_in_different_calls()
        {
            DateTimeStrategyHolder.Current = new DefaultDateTimeStrategy();
            var firstTime = BusinessDateTime.Now;
            Thread.Sleep(5);
            var secondTime = BusinessDateTime.Now;
            Assert.True(firstTime != secondTime);
        }
    }
}