using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    public class SnapshotsSavePolicy
    {
        private int _messagesProduced;
        private DateTime _lastActivityTime;
        private readonly TimeSpan _sleepTime;
        private readonly int _saveOnEach;

        public SnapshotsSavePolicy(TimeSpan sleepTime, int saveOnEach)
        {
            _saveOnEach = saveOnEach;
            _sleepTime = sleepTime;
        }


        public bool ShouldSave(params object[] stateChanges)
        {
            if (!stateChanges.Any()) return false;

            if(_messagesProduced == 0 && _lastActivityTime == default(DateTime))
               RefreshActivity();

            _messagesProduced += stateChanges.Length;
            if ((_messagesProduced % _saveOnEach == 0) || _lastActivityTime + _sleepTime < BusinessDateTime.UtcNow)
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