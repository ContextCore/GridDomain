using System;
using GridDomain.Common;

namespace GridDomain.Node.Actors
{
    class SnapshotsSavePolicy
    {
        private readonly Action _save;
        private int _messagesProduced;
        private DateTime _lastActivityTime = BusinessDateTime.UtcNow;
        private readonly TimeSpan _sleepTime;
        private readonly int _saveOnEach;

        public SnapshotsSavePolicy(Action save, TimeSpan? sleepTime = null, int saveOnEach = 10)
        {
            _saveOnEach = saveOnEach;
            _save = save;
            _sleepTime = sleepTime ?? TimeSpan.FromMinutes(1);
        }

        public bool TrySave(object lastMessage)
        {
            if (++_messagesProduced%_saveOnEach != 0 && _lastActivityTime + _sleepTime >= BusinessDateTime.UtcNow)
            {
                RefreshActivity();
                return false;
            }

            _save();
            return true;
        }

        public void RefreshActivity(DateTime? lastActivityTime = null)
        {
            _lastActivityTime = lastActivityTime ?? BusinessDateTime.UtcNow;
        }
    }
}