using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Tests.Common;
using Xunit;

namespace GridDomain.Tests.Unit
{
    public class DateTimeSpec
    {
        [Fact]
        public async Task When_DefaultDateTimeStrategy_is_used_Then_DateTimeNow_should_differ_in_different_calls()
        {
            DateTimeStrategyHolder.Current = new DefaultDateTimeStrategy();
            var firstTime = BusinessDateTime.Now;
            await Task.Delay(5);
            var secondTime = BusinessDateTime.Now;
            Assert.NotEqual(firstTime, secondTime);
        }

        [Fact]
        public void When_EditableDateTimeStrategy_is_used_and_Now_is_set_Then_DateTimeFacadeNow_should_be_the_same()
        {
            var testTime = DateTime.Now.AddSeconds(-3);
            DateTimeStrategyHolder.Current = new TestDateTimeStrategy {EditableNow = testTime};
            var now = BusinessDateTime.Now;
            Assert.Equal(now, testTime);
        }
    }
}