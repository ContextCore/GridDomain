using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Util.Internal;
using GridDomain.Configuration;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Tests.Common;
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

        [Fact]
        public void Given_policy_with_keep_When_saved_many_Then_only_keep_amount_is_left()
        {
            var policy = new SnapshotsPersistencePolicy(1, 2);
            SnapshotSelectionCriteria deleteCriteria;

            CheckSnapshotSaved(policy, 1);
            //should not delete as keep limit 2 was not reached
            Assert.False(policy.ShouldDelete(out deleteCriteria));

            CheckSnapshotSaved(policy, 2);
            //should not delete as keep limit 2 was not exceeded
            Assert.False(policy.ShouldDelete(out deleteCriteria));

            CheckSnapshotSaved(policy, 3);
            //should delete as keep limit 2 was exceeded by 1
            Assert.True(policy.ShouldDelete(out deleteCriteria));
            Assert.Equal(1, deleteCriteria.MaxSequenceNr);

            CheckSnapshotSaved(policy, 4);
            //should delete as keep limit 2 was exceeded by 2
            Assert.True(policy.ShouldDelete(out deleteCriteria));
            Assert.Equal(2, deleteCriteria.MaxSequenceNr);

            CheckSnapshotSaved(policy, 5);
            //should delete as keep limit 2 was exceeded by 3
            Assert.True(policy.ShouldDelete(out deleteCriteria));
            Assert.Equal(3, deleteCriteria.MaxSequenceNr);
        }

        [Fact]
        public void Given_policy_with_keep_When_saved_many_with_wrong_confirmation_order_Then_only_keep_amount_is_left()
        {
            var policy = new SnapshotsPersistencePolicy(1, 2);
            SnapshotSelectionCriteria deleteCriteria;

            var confirmationSequence = new[] {1, 2, 3, 4, 5};
            confirmationSequence.Shuffle();

            confirmationSequence.ForEach(s => 
                            policy.MarkSnapshotSaving());

            confirmationSequence.ForEach(s =>
                                             policy.MarkSnapshotSaved(s));
            
            //should delete as keep limit 2 was exceeded by 3
            Assert.True(policy.ShouldDelete(out deleteCriteria));
            Assert.Equal(3, deleteCriteria.MaxSequenceNr);
        }

        private static void CheckSnapshotSaved(SnapshotsPersistencePolicy policy, int snapshotsSequenceNumber)
        {
            Assert.True(policy.ShouldSave(snapshotsSequenceNumber));
            policy.MarkSnapshotSaving();
            policy.MarkSnapshotSaved(snapshotsSequenceNumber);
        }
    }
}
