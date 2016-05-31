using System;
using System.Globalization;
using System.Linq;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Tests.Scheduling.TestHelpers;
using Moq;
using NUnit.Framework;
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
        private Mock<IQuartzLogger> _quartzLogger;

        [SetUp]
        public void SetUp()
        {
            _loggingSchedulerListener = new Mock<ILoggingSchedulerListener>();
            _loggingJobListener = new Mock<ILoggingJobListener>();
            _loggingJobListener.Setup(x => x.Name).Returns("testListener");
            var quartzConfig = new QuartzConfig();
            _quartzScheduler = new SchedulerFactory(quartzConfig, _loggingSchedulerListener.Object, _loggingJobListener.Object).Create();
            _quartzLogger = new Mock<IQuartzLogger>();
            QuartzLoggerFactory.SetLoggerFactory(() => _quartzLogger.Object);
            _scheduler = Sys.ActorOf(Props.Create(() => new SchedulerActor(_quartzScheduler)));
            _taskRouter = new TaskRouter();
            TaskRouterFactory.Init(_taskRouter);
            _quartzScheduler.Clear();
        }

        [Test]
        public void When_job_is_added_Then_it_gets_executed()
        {
            var runAt = DateTime.UtcNow.AddSeconds(1);
            var testRequest = new TestRequest();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestRequestHandler>();
            _taskRouter.AddRoute(testRequest.GetType(), testActor);
            _scheduler.Ask<TaskAdded>(new AddTask(testRequest, runAt, Timeout)).Wait(Timeout);
            Throttle.Verify(() => _quartzLogger.Verify(x => x.LogSuccess(testRequest.TaskId)), Timeout);
        }

        [Test]
        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {
            var runAt = DateTime.UtcNow.AddSeconds(1);
            var testRequest = new TestRequest();
            var testActor = ActorOfAsTestActorRef<FailingTestRequestHandler>();
            _taskRouter.AddRoute(testRequest.GetType(), testActor);
            _scheduler.Tell(new AddTask(testRequest, runAt, Timeout));
            Throttle.Verify(() => _quartzLogger.Verify(x => x.LogFailure(testRequest.TaskId, It.IsAny<Exception>())), Timeout);
        }

        [Test]
        public void When_there_are_several_scheduled_jobs_System_executes_all_of_them()
        {
            var tasks = new[] { 0.5, 1, 1.5, 2, 2.5 };

            var testActor = ActorOfAsTestActorRef<SuccessfulTestRequestHandler>();
            _taskRouter.AddRoute(typeof(TestRequest), testActor);

            foreach (var task in tasks)
            {
                var testRequest = new TestRequest(task.ToString(CultureInfo.InvariantCulture));
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new AddTask(testRequest, runAt, Timeout));
            }

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => ResultHolder.Contains(taskIds));
        }

        [Test]
        public void When_some_of_scheduled_jobs_fail_System_still_executes_other()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var failTasks = new[] { 1.0, 2.0 };

            var testActor = ActorOfAsTestActorRef<SuccessfulTestRequestHandler>();
            _taskRouter.AddRoute(typeof(TestRequest), testActor);

            foreach (var task in successTasks.Concat(failTasks))
            {
                var testRequest = new TestRequest(task.ToString(CultureInfo.InvariantCulture));
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new AddTask(testRequest, runAt, Timeout));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var failTaskIds = failTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();


            Throttle.Assert(() =>
            {
                ResultHolder.Contains(successTaskIds);
                Assert.True(failTaskIds.All(x => ResultHolder.Get(x) == null));
            });
        }



        private static TimeSpan Timeout
        {
            get
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
}