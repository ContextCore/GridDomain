namespace Automatonymous.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class When_combining_events_with_an_int_for_state
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance() {CurrentState = nameof(TestStateMachine.Waiting)};
            // await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.IsFalse(_instance.Called);

            await _machine.RaiseEvent(_instance, _machine.First, 1);
            await _machine.RaiseEvent(_instance, _machine.Second, "test");

            Assert.IsTrue(_instance.Called);

            Assert.AreEqual(nameof(_machine.Final), _instance.CurrentState);
        }

        [Test]
        public async Task Should_have_called_combined_event_on_any_order()
        {
            _machine = new TestStateMachine();
            _instance = new Instance() {CurrentState = nameof(TestStateMachine.Waiting)};
            // await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.IsFalse(_instance.Called);

            await _machine.RaiseEvent(_instance, _machine.Second, "test");
            await _machine.RaiseEvent(_instance, _machine.First, 1);


            Assert.IsTrue(_instance.Called);

            Assert.AreEqual(nameof(_machine.Final), _instance.CurrentState);
        }


        TestStateMachine _machine;
        Instance _instance;
    }
}