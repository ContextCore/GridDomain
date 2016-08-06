using System;
using GridDomain.Logging;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools;
using NUnit.Framework;

namespace Solomoto.Membership.Tools
{
    [TestFixture]
    public class LoadAggregateTest
    {
        [Test]
        public void Test_business()
        {
            using (var repo = new Repository(new AkkaEventRepository(new AutoTestAkkaConfiguration())))
            {
                var id = Guid.Parse("07be4567-7828-4e98-840a-e560a4293c7e");
              //  var aggregate = repo.LoadAggregate<BusinessSubscriptionAggregate>(id);
              //  Console.WriteLine(aggregate.ToPropsString());
            }
        }
    }
}