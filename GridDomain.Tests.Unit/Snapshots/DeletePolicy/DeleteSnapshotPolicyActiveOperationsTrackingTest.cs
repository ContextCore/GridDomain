using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using Xunit;

namespace GridDomain.Tests.Unit.Snapshots.DeletePolicy {
    public class DeleteSnapshotPolicyActiveOperationsTrackingTest
    {
        [Fact]
        public void Given_policy_When_delete_complete_with_previos_delete_pending_Then_they_should_be_treated_as_completed()
        {
            var policy = new SnapshotsDeletePolicy(5);
            policy.Tracking.Start(new SnapshotSelectionCriteria(3));
            policy.Tracking.Start(new SnapshotSelectionCriteria(4));
            policy.Tracking.Complete(new SnapshotSelectionCriteria(7));
            Assert.Equal(0,policy.Tracking.InProgress);
        }

        [Fact]
        public void Given_policy_When_delete_starts_Then_it_should_increment_active_count()
        {
            var policy = new SnapshotsDeletePolicy(5);
            policy.Tracking.Start(new SnapshotSelectionCriteria(3));
            Assert.Equal(1,policy.Tracking.InProgress);
            policy.Tracking.Start(new SnapshotSelectionCriteria(4));
            Assert.Equal(2,policy.Tracking.InProgress);
        }

        [Fact]
        public void Given_policy_When_delete_completes_Then_it_should_decrement_active_count()
        {
            var policy = new SnapshotsDeletePolicy(5);
            policy.Tracking.Start(new SnapshotSelectionCriteria(3));
            Assert.Equal(1,policy.Tracking.InProgress);
            policy.Tracking.Complete(new SnapshotSelectionCriteria(3));
            Assert.Equal(0,policy.Tracking.InProgress);
        } 
         
        [Fact]
        public void Given_policy_When_delete_fails_Then_it_should_decrement_active_count()
        {
            var policy = new SnapshotsDeletePolicy(5);
            policy.Tracking.Start(new SnapshotSelectionCriteria(3));
            Assert.Equal(1,policy.Tracking.InProgress);
            policy.Tracking.Fail(new SnapshotSelectionCriteria(3));
            Assert.Equal(0,policy.Tracking.InProgress);
        } 
        
        [Fact]
        public void Given_policy_When_delete_fails_with_previos_save_pending_Then_it_should_not_finish_them()
        {
            var policy = new SnapshotsDeletePolicy(5);
            policy.Tracking.Start(new SnapshotSelectionCriteria(3));
            policy.Tracking.Start(new SnapshotSelectionCriteria(4));
            policy.Tracking.Fail(new SnapshotSelectionCriteria(4));
            Assert.Equal(1,policy.Tracking.InProgress);
        }
        
        [Fact]
        public void Given_policy_When_try_to_delete_similar_number_several_times_Then_it_should_increment_active_count_only_once()
        {
            var policy = new SnapshotsDeletePolicy(5);
            policy.Tracking.Start(new SnapshotSelectionCriteria(3));
            policy.Tracking.Start(new SnapshotSelectionCriteria(3));
            Assert.Equal(1,policy.Tracking.InProgress);
        } 
        
    }
}