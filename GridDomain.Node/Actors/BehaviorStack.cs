using System;
using System.Collections.Generic;
using System.Text;

namespace GridDomain.Node.Actors
{
    public class BehaviorStack : BehaviorBag
    {
        private readonly Stack<string> _behaviorStack = new Stack<string>();
        public IReadOnlyCollection<string> Stack => _behaviorStack;

        public BehaviorStack(Action<Action> become, Action unbecome, int historyLimit = 10):base(become,unbecome,historyLimit)
        {
        }

        public override void Become(Action act, string name)
        {
            _behaviorStack.Push(name);
            base.Become(act,name);
        }


        public override string Unbecome()
        {
            _behaviorStack.Pop();
            return base.Unbecome();
        }
    }
}