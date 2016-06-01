using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.TestKit.NUnit;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Tests.Scheduling.TestHelpers;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Unity;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Scheduling
{
    [TestFixture]
    public class AkkaSpec : TestKit
    {
        private IActorRef _scheduler;
        private IScheduler _quartzScheduler;
        private Mock<IQuartzLogger> _quartzLogger;
        private UnityContainer _container;
        private ITaskRouter _taskRouter;

        private UnityContainer Register()
        {
            var container = new UnityContainer();
            container.AddNewExtension<QuartzUnityExtension>();
            var loggingJobListener = new Mock<ILoggingJobListener>();
            loggingJobListener.Setup(x => x.Name).Returns("testListener");
            //container.RegisterInstance(loggingJobListener.Object);
            container.RegisterType<ILoggingJobListener, LoggingJobListener>();
            //container.RegisterInstance(new Mock<ILoggingSchedulerListener>().Object);
            container.RegisterType<ILoggingSchedulerListener, LoggingSchedulerListener>();
            container.RegisterType<IQuartzConfig, QuartzConfig>();
            
            _taskRouter = new TaskRouter();
            container.RegisterInstance(_taskRouter);
            _quartzLogger = new Mock<IQuartzLogger>();
            container.RegisterInstance(_quartzLogger.Object);
            container.RegisterType<SchedulerActor>();
            container.RegisterType<ISchedulerFactory, SchedulerFactory>();
            container.RegisterType<IScheduler>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<ISchedulerFactory>().GetScheduler()));
            container.RegisterType<QuartzJob>();
            container.RegisterType<SuccessfulTestRequestHandler>();
            container.RegisterType<FailingTestRequestHandler>();
            container.RegisterType(typeof(TestRequestHandler<>));
            return container;
        }

        [SetUp]
        public void SetUp()
        {
            _container = Register();
            Sys.AddDependencyResolver(new UnityDependencyResolver(_container, Sys));
            CreateScheduler();

            _scheduler = Sys.ActorOf(Sys.DI().Props<SchedulerActor>());
            _quartzScheduler.Clear();
            ResultHolder.Clear();
        }

        private void CreateScheduler()
        {
            _quartzScheduler = _container.Resolve<ISchedulerFactory>().GetScheduler();
        }

        [Test]
        public void When_system_resolves_scheduler_Then_single_instance_is_returned_in_all_cases()
        {
            var sched1 = _container.Resolve<IScheduler>();
            var sched2 = _container.Resolve<IScheduler>();
            Assert.True(sched2 == sched1);
        }

        [Test]
        public void When_job_is_added_Then_it_gets_executed()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testRequest = new TestRequest();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestRequestHandler>();
            _taskRouter.AddRoute(testRequest.GetType(), testActor);
            _scheduler.Ask<TaskAdded>(new AddTask(testRequest, runAt, Timeout)).Wait(Timeout);
            Throttle.Verify(_quartzLogger, x => x.LogSuccess(testRequest.TaskId), maxTimeout: Timeout);
        }

        [Test]
        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testRequest = new FailTaskRequest("taskId");
            var testActor = ActorOfAsTestActorRef<FailingTestRequestHandler>();
            _taskRouter.AddRoute(testRequest.GetType(), testActor);
            _scheduler.Tell(new AddTask(testRequest, runAt, Timeout));
            Throttle.Verify(_quartzLogger, x => x.LogFailure(testRequest.TaskId, It.IsAny<Exception>()), maxTimeout: Timeout);
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
        public void When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var secondRunAt = DateTime.UtcNow.AddSeconds(1);
            var taskId = "taskId";
            var result = new List<string>();

            Action<TestRequest> handler = request => result.Add(request.TaskId);

            var testRequest = new TestRequest(taskId);
            var testActor = ActorOfAsTestActorRef<TestRequestHandler<TestRequest>>(Props.Create(() => new TestRequestHandler<TestRequest>(handler)));
            _taskRouter.AddRoute(typeof(TestRequest), testActor);
            _scheduler.Tell(new AddTask(testRequest, runAt, Timeout));
            _scheduler.Tell(new AddTask(testRequest, secondRunAt, Timeout));

            Throttle.Assert(() => Assert.True(result.Count == 1), minTimeout: TimeSpan.FromSeconds(2));
        }

        [Test]
        public void When_some_of_scheduled_jobs_fail_System_still_executes_others()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var failTasks = new[] { 1.0, 2.0 };

            var testActor = ActorOfAsTestActorRef<SuccessfulTestRequestHandler>();
            _taskRouter.AddRoute(typeof(TestRequest), testActor);

            foreach (var task in successTasks)
            {
                var testRequest = new TestRequest(task.ToString(CultureInfo.InvariantCulture));
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new AddTask(testRequest, runAt, Timeout));
            }
            foreach (var failTask in failTasks)
            {
                var failRequest = new FailTaskRequest(failTask.ToString(CultureInfo.InvariantCulture));
                var runAt = DateTime.UtcNow.AddSeconds(failTask);
                _scheduler.Tell(new AddTask(failRequest, runAt, Timeout));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var failTaskIds = failTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() =>
            {
                ResultHolder.Contains(successTaskIds);
                Assert.True(failTaskIds.All(x => ResultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(3));
        }

        [Test]
        public void When_tasks_get_deleted_after_scheduling_System_will_not_execute_them()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var tasksToRemove = new[] { 1.0, 2.0 };

            var testActor = ActorOfAsTestActorRef<SuccessfulTestRequestHandler>();
            _taskRouter.AddRoute(typeof(TestRequest), testActor);

            foreach (var task in successTasks.Concat(tasksToRemove))
            {
                var testRequest = new TestRequest(task.ToString(CultureInfo.InvariantCulture));
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new AddTask(testRequest, runAt, Timeout));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var tasksToRemoveTaskIds = tasksToRemove.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            foreach (var taskId in tasksToRemoveTaskIds)
            {
                _scheduler.Tell(new RemoveTask(taskId));
            }

            Throttle.Assert(() =>
            {
                ResultHolder.Contains(successTaskIds);
                Assert.True(tasksToRemoveTaskIds.All(x => ResultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(3));
        }

        [Test]
        public void When_scheduler_is_restarted_Then_scheduled_jobs_still_get_executed()
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

            _quartzScheduler.Shutdown(false);
            Thread.Sleep(2000);
            CreateScheduler();

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => ResultHolder.Contains(taskIds));
        }

        private static TimeSpan Timeout
        {
            get
            {
                //default timeout
                var timeout = TimeSpan.FromSeconds(3);
                //in case you want to debug this unit test
                if (Debugger.IsAttached)
                {
                    timeout = TimeSpan.FromSeconds(300);
                }
                return timeout;
            }
        }
    }
}