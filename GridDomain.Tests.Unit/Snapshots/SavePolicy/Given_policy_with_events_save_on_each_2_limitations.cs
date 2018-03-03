using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using Xunit;

namespace GridDomain.Tests.Unit.Snapshots.SavePolicy {
    
    public class Given_policy_with_events_save_on_each_2_limitations
    {
        SnapshotsSavePolicy Given()
        {
            return new SnapshotsSavePolicy(2);
        }
            
        [Fact]
        public void When_saving_many_events_with_confirmation_Then_should_save_only_each_second()
        {
            var policy = Given();
            Assert.True(policy.ShouldSave(1));
            policy.Tracking.Start(1);
            policy.Tracking.Complete(1);
                
            Assert.False(policy.ShouldSave(1));
            Assert.False(policy.ShouldSave(2));
            Assert.True(policy.ShouldSave(3));
        } 
            
        [Fact]
        public void When_saving_many_events_without_confirmation_Then_should_save_only_each_second()
        {
            var policy = Given();
            Assert.True(policy.ShouldSave(1));
            policy.Tracking.Start(1);
                
            Assert.False(policy.ShouldSave(1));
            Assert.False(policy.ShouldSave(2));
            //should approve saving of 3d event even without confirmation of event 1 save complete 
            //for cases when we lost confirmation somehow
            Assert.True(policy.ShouldSave(3));
        }
        
       
    }
}