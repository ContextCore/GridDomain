using GridDomain.Configuration;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using Xunit;

namespace GridDomain.Tests.Unit.Snapshots.DeletePolicy {
    public class Given_policy_with_keep_events_number_limitation
    {

        SnapshotsDeletePolicy Given()
        {
            return new SnapshotsDeletePolicy(2);
        }
        
        [Fact]
        public void When_deleting_Then_should_keep_requested_amount_of_events()
        {
            var policy = Given();
            policy.ShouldDelete(10,out SnapshotSelectionCriteria criteria);
            Assert.Equal(8, criteria.MaxSequenceNr);
        }
            
        [Fact]
        public void When_delete_in_progress_and_trying_to_delete_greater_snapshotNumber_Then_should_delete_without_confirmation()
        {
            var policy = Given();
            policy.Tracking.Start(new SnapshotSelectionCriteria(1));
            policy.Tracking.Start(new SnapshotSelectionCriteria(5));
            Assert.True(policy.ShouldDelete(10,out SnapshotSelectionCriteria criteria));
        }
        
        [Fact]
        public void When_delete_in_progress_and_trying_to_delete_lower_snapshotNumber_Then_should_not_delete()
        {
            var policy = Given();
            policy.Tracking.Start(new SnapshotSelectionCriteria(1));
            policy.Tracking.Start(new SnapshotSelectionCriteria(5));
            Assert.False(policy.ShouldDelete(3,out SnapshotSelectionCriteria criteria));
        }
            
        [Fact]
        public void When_delete_fails_Then_should_delete_on_same_and_next_sequence()
        {
            var policy = Given();
            policy.Tracking.Start(new SnapshotSelectionCriteria(1));
            policy.Tracking.Start(new SnapshotSelectionCriteria(5));
            policy.Tracking.Fail(new SnapshotSelectionCriteria(5));
            
            Assert.True(policy.ShouldDelete(4,out SnapshotSelectionCriteria criteria));
            Assert.Equal(2, criteria.MaxSequenceNr);
            Assert.True(policy.ShouldDelete(7,out criteria));
            Assert.Equal(5, criteria.MaxSequenceNr);
        }
    }
}