using System;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using Xunit;

namespace GridDomain.Tests.Unit.Snapshots.DeletePolicy {
    public class Given_policy_with_keep_events_for_a_time_limitation
    {
        [Fact]
        public void When_trying_to_delete_Then_should_keep_events_according_to_its_lifetime()
        {
            var policy = new SnapshotsDeletePolicy(1, TimeSpan.FromSeconds(10));
            Assert.True(policy.ShouldDelete(5,out SnapshotSelectionCriteria criteria));
            var expectedTimeStamp = BusinessDateTime.UtcNow - policy.MaxSnapshotAge;
            Assert.True((expectedTimeStamp - criteria.MaxTimeStamp) <= TimeSpan.FromMilliseconds(100));
        }
    }
}