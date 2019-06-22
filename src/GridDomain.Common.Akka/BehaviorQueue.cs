using System;
using System.Collections.Generic;
using Akka.Actor;

namespace GridDomain.Common.Akka {
    public class BehaviorQueue
    {
        private readonly Queue<string> _behaviorHistory;
        private readonly Action<Action> _become;
        private readonly int _historyLimit;
        public string Current { get; private set; }
        public IReadOnlyCollection<string> History => _behaviorHistory;

        public BehaviorQueue(Action<Action> become, int historyLimit = 10)
        {
            _historyLimit = historyLimit;
            _behaviorHistory = new Queue<string>(historyLimit+1);
            _behaviorHistory.Enqueue("Default behavior");
            _become = become;
        }

        public void Become(Action act, string name)
        {
            Current = name;
            Remember(name);
            _become(act);
        }

        private void Remember(string name)
        {
            _behaviorHistory.Enqueue(name);
            if (_behaviorHistory.Count >= _historyLimit)
                _behaviorHistory.Dequeue();
        }
    }

    public static class ActorSystemExtensions
    {
        public static Address GetAddress(this ActorSystem system)
        {
            return ((ExtendedActorSystem) system).Provider.DefaultAddress;
        }
    }
}