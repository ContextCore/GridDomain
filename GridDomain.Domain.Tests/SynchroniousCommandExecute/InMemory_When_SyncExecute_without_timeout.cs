using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_When_SyncExecute_without_timeout : SampleDomainCommandExecutionTests
    {

        public InMemory_When_SyncExecute_without_timeout() : base(true)
        {

        }

        [Then]
        public void PlanExecute_throw_exception_after_wait_without_timeout()
        {
            var syncCommand = new LongOperationCommand(1000,Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var plan = CommandPlan.New(syncCommand, TimeSpan.FromMilliseconds(500), expectedMessage);

            Assert_TimeoutException_In_inner_exceptions(() => GridNode.Execute(plan).Wait());
        }

        [Then]
        public void SyncExecute_throw_exception_after_wait_without_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            GridNode.CommandTimeout = TimeSpan.FromMilliseconds(500);

            Assert_TimeoutException_In_inner_exceptions(() => GridNode.Execute(syncCommand, expectedMessage).Wait());
        }

        private static void Assert_TimeoutException_In_inner_exceptions(Action act)
        {
            try
            {
                act.Invoke();
                Assert.Fail("Timeout exception was not raised");
            }
            catch (AggregateException ex)
            {
                var e = ex.InnerExceptions.First();
                Assert.IsInstanceOf<TimeoutException>(e);
            }
        }

        [Then]
        public void PlanExecute_by_result_throws_exception_after_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var plan = CommandPlan.New(syncCommand, TimeSpan.FromMilliseconds(500), expectedMessage);
            
            Assert_TimeoutException_In_inner_exceptions(() => { object A = GridNode.Execute(plan).Result; });
        }


        [Then]
        public void SyncExecute_by_result_throws_exception_after_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            GridNode.CommandTimeout = TimeSpan.FromMilliseconds(500);

            Assert_TimeoutException_In_inner_exceptions(() => { object A = GridNode.Execute(syncCommand, expectedMessage).Result; });
        }


        [Then]
        public void PlanExecute_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var plan = CommandPlan.New(syncCommand, expectedMessage);
            GridNode.CommandTimeout = TimeSpan.FromMilliseconds(500);

            Assert.False(GridNode.Execute(plan).Wait(100));
        }

        [Then]
        public void SyncExecute_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            GridNode.CommandTimeout = TimeSpan.FromMilliseconds(500);

            Assert.False(GridNode.Execute(syncCommand, expectedMessage).Wait(100));
        }



    }
}