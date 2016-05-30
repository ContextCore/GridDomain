using System;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Akka.Tasks;
using GridDomain.Tests.Scheduling.TestHelpers;
using Moq;
using NUnit.Framework;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Scheduling
{
    [TestFixture]
    public class AkkaSpec : TestKit
    {
        private IActorRef _scheduler;
        private TaskRouter _taskRouter;
        private IScheduler _quartzScheduler;
        private Mock<ILoggingSchedulerListener> _loggingSchedulerListener;
        private Mock<ILoggingJobListener> _loggingJobListener;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var quartzConfig = new QuartzConfig();
            _loggingSchedulerListener = new Mock<ILoggingSchedulerListener>();
            _loggingJobListener = new Mock<ILoggingJobListener>();
            _loggingJobListener.Setup(x => x.Name).Returns("testListener");
            _quartzScheduler = new SchedulerFactory(quartzConfig, _loggingSchedulerListener.Object, _loggingJobListener.Object).Create();
            _scheduler = Sys.ActorOf(Props.Create(() => new SchedulerActor(_quartzScheduler)));
            _taskRouter = new TaskRouter();
            TaskRouterFactory.Init(_taskRouter);
        }

        [SetUp]
        public void SetUp()
        {
            _quartzScheduler.Clear();
        }

        [Test]
        public void When_job_is_triggered_Then_job_processing_actor_gets_the_message()
        {
            var runAt = DateTime.UtcNow.AddSeconds(1);
            var timeout = GetTimeout();
            var testRequest = new TestRequest();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestRequestHandler>();
            _taskRouter.AddRoute(testRequest.GetType(), testActor);
            _scheduler.Ask<TaskAdded>(new AddTask(new AkkaScheduledTask(runAt, testRequest))).Wait(timeout);
            ExpectMsg<TestRequest>(timeout);
            _loggingJobListener.Verify(x => x.JobWasExecuted(It.IsAny<IJobExecutionContext>(), It.IsAny<JobExecutionException>()));
        }


        public void When_processing_actor_finishes_job_Then_it_sends_response_to_scheduler()
        {

        }

        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {

        }


        private static TimeSpan GetTimeout()
        {
            //default timeout
            var timeout = TimeSpan.FromSeconds(3);
#if DEBUG
            //in case you want to debug this unit test
            timeout = TimeSpan.FromSeconds(300);
#endif
            return timeout;
        }
    }
}