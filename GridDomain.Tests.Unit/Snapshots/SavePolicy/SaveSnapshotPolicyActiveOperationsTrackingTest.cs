using System;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using Xunit;

namespace GridDomain.Tests.Unit.Snapshots.SavePolicy {
    public class SaveSnapshotPolicyActiveOperationsTrackingTest
    {
        [Fact]
        public void Given_policy_When_save_complete_with_previos_save_pending_Then_they_should_be_treated_as_completed()
        {
            var policy = new SnapshotsSavePolicy(2);
            policy.Tracking.Start(1);
            policy.Tracking.Start(2);
            policy.Tracking.Start(3);
            policy.Tracking.Complete(3);
            //we dont care about previos snapshots save if we saved the latest one 
            Assert.Equal(0, policy.Tracking.InProgress);
        }
        
        [Fact]
        public void Given_policy_When_save_starts_Then_it_should_increment_active_count()
        {
            var policy = new SnapshotsSavePolicy(2);
            policy.Tracking.Start(1);
            Assert.Equal(1, policy.Tracking.InProgress);
            policy.Tracking.Start(1);
            Assert.Equal(1, policy.Tracking.InProgress);
        } 
        
        [Fact]
        public void Given_policy_When_try_to_save_similar_number_several_times_Then_it_should_increment_active_count_only_once()
        {
            var policy = new SnapshotsSavePolicy(2);
            policy.Tracking.Start(1);
            policy.Tracking.Start(1);
            policy.Tracking.Start(1);
            policy.Tracking.Start(1);
            Assert.Equal(1, policy.Tracking.InProgress);
        } 
        
        [Fact]
        public void Given_policy_When_save_completes_Then_it_should_decrement_active_count()
        {
            var policy = new SnapshotsSavePolicy(2);
            policy.Tracking.Start(1);
            policy.Tracking.Start(2);
            policy.Tracking.Complete(1);
            Assert.Equal(1, policy.Tracking.InProgress);
        } 
           
        [Fact]
        public void Given_policy_When_save_fails_Then_it_should_decrement_active_count()
        {
            var policy = new SnapshotsSavePolicy(2);
            policy.Tracking.Start(1);
            policy.Tracking.Start(2);
            policy.Tracking.Fail(1);
            Assert.Equal(1, policy.Tracking.InProgress);
        } 
        
        [Fact]
        public void Given_policy_When_save_fails_with_previos_save_pending_Then_it_should_not_finish_them()
        {
            var policy = new SnapshotsSavePolicy(2);
            policy.Tracking.Start(1);
            policy.Tracking.Start(2);
            policy.Tracking.Fail(2);
            Assert.Equal(1, policy.Tracking.InProgress);
        } 
    }
}