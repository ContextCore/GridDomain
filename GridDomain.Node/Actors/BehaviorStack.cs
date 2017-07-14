using System;
using System.Collections.Generic;
using System.Text;

namespace GridDomain.Node.Actors
{
    public class BehaviorStack:BehaviorQueue
    {
        private readonly Action _unbecome;
        public IReadOnlyCollection<string> Stack => _behaviorStack;
        private readonly Stack<string> _behaviorStack = new Stack<string>();

        public BehaviorStack(Action<Action> become, Action unbecome, int historyLimit = 10):base(become,historyLimit)
        {
            _unbecome = unbecome;
            _behaviorStack.Push("Default behavior");
        }

        public override void Become(Action act, string name)
        {
            _behaviorStack.Push(name);
            base.Become(act, name);
        }

        public void Unbecome()
        {
            _unbecome();
            _behaviorStack.Pop();
            Current = _behaviorStack.Peek();
            Remember(Current);
        }
    }
}