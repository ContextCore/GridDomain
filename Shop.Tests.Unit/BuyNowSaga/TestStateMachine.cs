namespace Automatonymous.Tests
{
    public sealed class TestStateMachine :
        AutomatonymousStateMachine<Instance>
    {
        public TestStateMachine()
        {
            InstanceState(x => x.CurrentState);

            CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

            During(Waiting,
                When(First)
                    .Then(context => context.Instance.FirstCalled = true),
                When(Second)
                    .Then(context => context.Instance.SecondCalled = true),
                When(Third)
                    .Then(context => context.Instance.Called = true)
                    .Finalize());
        }

        public State Waiting { get; private set; }

        public Event<int> First { get; private set; }
        public Event<string> Second { get; private set; }
        public Event Third { get; private set; }
    }
}