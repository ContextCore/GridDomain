using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node.Actors.EventSourced;
using Xunit;

namespace GridDomain.Tests.Unit
{
    public class SnapshotsSavePolicyTests
    {
        [Fact]
        public async Task Given_policy_with_frequency_limitations_When_saved_to_frequently_Then_only_should_save_is_true_after_time_passed()
        {
            var policy = new SnapshotsPersistencePolicy(1,10,TimeSpan.FromSeconds(1));

            Assert.True(policy.ShouldSave(1));
            policy.MarkSnapshotSaving();
            Assert.False(policy.ShouldSave(2));
            Assert.False(policy.ShouldSave(1));
            await Task.Delay(policy.MaxSaveFrequency);
            Assert.True(policy.ShouldSave(2));
        }
    }
}
