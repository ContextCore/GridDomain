using System;
using Akka.Persistence;

namespace GridDomain.Node.Actors
{
    public interface ISnapshotsSavePolicy
    {
        void MarkActivity(DateTime? lastActivityTime = default(DateTime?));
        bool ShouldSave(params object[] stateChanges);
        SnapshotSelectionCriteria GetSnapshotsToDelete();
        void MarkSnapshotApplied(SnapshotMetadata metadata);
        void MarkSnapshotSaved(SnapshotMetadata metadata);
    }
}