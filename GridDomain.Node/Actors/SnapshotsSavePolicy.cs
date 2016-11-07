using System;
using GridDomain.Common;

namespace GridDomain.Node.Actors
{
    public class SnapshotsSavePolicy
    {
        private int _messagesProduced;
        private DateTime _lastActivityTime = BusinessDateTime.UtcNow;
        private readonly TimeSpan _sleepTime;
        private readonly int _saveOnEach;

        public SnapshotsSavePolicy(TimeSpan sleepTime, int saveOnEach)
        {
            _saveOnEach = saveOnEach;
            _sleepTime = sleepTime;
        }

        public bool ShouldSave(object lastMessage)
        {
            if (++_messagesProduced%_saveOnEach == 0 || _lastActivityTime + _sleepTime < BusinessDateTime.UtcNow)
                return true;

            RefreshActivity();
            return false;
        }

        public void RefreshActivity(DateTime? lastActivityTime = null)
        {
            _lastActivityTime = lastActivityTime ?? BusinessDateTime.UtcNow;
        }
    }
}