using System;
using Akka.Persistence;

namespace GridDomain.Node.Actors
{
    public interface ISnapshotsSavePolicy
    {
        void RefreshActivity(DateTime? lastActivityTime = default(DateTime?));
        bool ShouldSave(params object[] stateChanges);
        SnapshotSelectionCriteria SnapshotsToDelete();
        void SnapshotWasApplied(SnapshotMetadata metadata);
        void SnapshotWasSaved(SnapshotMetadata metadata);
    }
}