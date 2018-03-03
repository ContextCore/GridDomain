using System;
using System.Threading.Tasks;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using Xunit;

namespace GridDomain.Tests.Unit.Snapshots.SavePolicy
{
     public class Given_policy_with_frequency_limitations
        {
            SnapshotsSavePolicy Given()
            {
                 return new SnapshotsSavePolicy(1,TimeSpan.FromSeconds(1));
            }

            [Fact]
            public async Task When_saved_too_frequently_with_confirmation_Then_should_save_after_time_passed()
            {
                var policy = Given();
                Assert.True(policy.ShouldSave(1));
                policy.Tracking.Start(1);
                policy.Tracking.Complete(1);
                Assert.False(policy.ShouldSave(1));
                Assert.False(policy.ShouldSave(2));
                await Task.Delay(policy.MaxSaveFrequency);
                Assert.True(policy.ShouldSave(2));
            }
            
            [Fact]
            public async Task When_saved_too_frequently_and_persist_take_too_much_time_Then_should_save_before_confirmation()
            {
                var policy = Given();
                Assert.True(policy.ShouldSave(1));
                policy.Tracking.Start(1);
                Assert.False(policy.ShouldSave(1));
                Assert.False(policy.ShouldSave(2));
                await Task.Delay(policy.MaxSaveFrequency);
                Assert.True(policy.ShouldSave(2));
            }
        
            [Fact]
            public async Task When_persist_fails_Then_should_not_count_it_as_save_attempt_and_reset_time_limitation()
            {
                var policy = Given();
                Assert.True(policy.ShouldSave(1));
                policy.Tracking.Start(1);
                policy.Tracking.Fail(1);
                Assert.True(policy.ShouldSave(1));
                Assert.True(policy.ShouldSave(2));
            }
        }
}
