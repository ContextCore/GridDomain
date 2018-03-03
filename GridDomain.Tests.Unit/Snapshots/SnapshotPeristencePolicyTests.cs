using Akka.Util.Internal;
using GridDomain.Configuration;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using GridDomain.Tests.Common;
using Xunit;

namespace GridDomain.Tests.Unit.Snapshots {
    public class SnapshotPeristencePolicyTests
    {
        [Fact]
        public void Given_policy_with_keep_When_saved_many_Then_only_keep_amount_is_left()
        {
            ISnapshotsPersistencePolicy policy = new SnapshotsPersistencePolicy(1, null, 2);

            CheckSnapshotSaved(policy, 1);
            //should not delete as keep limit 2 was not reached
            Assert.False(policy.ShouldDelete(1, out var deleteCriteria));

            CheckSnapshotSaved(policy, 2);
            //should not delete as keep limit 2 was not exceeded
            Assert.False(policy.ShouldDelete(2,out deleteCriteria));

            CheckSnapshotSaved(policy, 3);
            //should delete as keep limit 2 was exceeded by 1
            Assert.True(policy.ShouldDelete(3,out deleteCriteria));
            Assert.Equal(1, deleteCriteria.MaxSequenceNr);

            CheckSnapshotSaved(policy, 4);
            //should delete as keep limit 2 was exceeded by 2
            Assert.True(policy.ShouldDelete(4,out deleteCriteria));
            Assert.Equal(2, deleteCriteria.MaxSequenceNr);

            CheckSnapshotSaved(policy, 5);
            //should delete as keep limit 2 was exceeded by 3
            Assert.True(policy.ShouldDelete(5,out deleteCriteria));
            Assert.Equal(3, deleteCriteria.MaxSequenceNr);
        }

        [Fact]
        public void Given_policy_with_keep_When_saved_many_with_wrong_confirmation_order_Then_only_keep_amount_is_left()
        {
            var policy = new SnapshotsPersistencePolicy(1, null, 2);

            var confirmationSequence = new[] {1, 2, 3, 4, 5};
            confirmationSequence.Shuffle();


            confirmationSequence.ForEach(s => policy.SavePolicy.Tracking.Complete(s));
            
            //should delete as keep limit 2 was exceeded by 3
            Assert.True(policy.DeletePolicy.ShouldDelete(5,out SnapshotSelectionCriteria deleteCriteria));
            Assert.Equal(3, deleteCriteria.MaxSequenceNr);
        }

        private static void CheckSnapshotSaved(ISnapshotsSavePolicy policy, int snapshotsSequenceNumber)
        {
            Assert.True(policy.ShouldSave(snapshotsSequenceNumber));
            policy.Tracking.Complete(snapshotsSequenceNumber);
        }
    }
}