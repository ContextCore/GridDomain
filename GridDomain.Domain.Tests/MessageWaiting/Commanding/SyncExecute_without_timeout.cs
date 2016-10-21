using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Commanding
{

    public static class ExceptionExtensions
    {
        public static void AssertInner<T>(this Exception e) where T: Exception
        {
            Assert.IsInstanceOf<T>(e.UnwrapSingle());
        }
    }

    [TestFixture]
    public class SyncExecute_without_timeout : SampleDomainCommandExecutionTests
    {
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            return new GridDomainNode(CreateConfiguration(),CreateMap(), () => new[]{akkaConf.CreateInMemorySystem() },
                new InMemoryQuartzConfig());
        }

        [Then]
        public void CommandWaiter_throws_exception_after_wait_with_obly_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000,Guid.NewGuid());
            var waiter = GridNode.NewCommandWaiter(TimeSpan.FromMilliseconds(100))
                                 .Expect<SampleAggregateChangedEvent>()
                                 .Create()
                                 .Execute(syncCommand);

            AssertInnerException<TimeoutException>(() => waiter.Wait());
        }

        [Then]
        public void SyncExecute_throw_exception_after_wait_without_timeout()
        {
            var syncCommand = new LongOperationCommand(1000000, Guid.NewGuid());
            var waiter = GridNode.NewCommandWaiter()
                                 .Expect<SampleAggregateChangedEvent>()
                                 .Create()
                                 .Execute(syncCommand);

            //default built-in timeout for 10 sec
            AssertInnerException<TimeoutException>(() => waiter.Wait());
        }

        [Then]
        public void SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000000, Guid.NewGuid());
            GridNode.DefaultTimeout = TimeSpan.FromMilliseconds(500);
            var waiter = GridNode.NewCommandWaiter()
                                 .Expect<SampleAggregateChangedEvent>()
                                 .Create()
                                 .Execute(syncCommand);

            AssertInnerException<TimeoutException>(() => waiter.Wait());
        }

        private static void AssertInnerException<T>(Action act) where T: Exception
        {
            try
            {
                act.Invoke();
                Assert.Fail("Timeout exception was not raised");
            }
            catch (Exception ex)
            {
                ex.AssertInner<T>();
            }
        }

        [Then]
        public void CommandWaiter_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            GridNode.NewCommandWaiter(TimeSpan.FromMilliseconds(100))
                                 .Expect<SampleAggregateChangedEvent>()
                                 .Create()
                                 .Execute(syncCommand)
                                 .Wait(100);
        }
    }
}