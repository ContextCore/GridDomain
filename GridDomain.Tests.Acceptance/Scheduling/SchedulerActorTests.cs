using System;
using System.Globalization;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Unit;
using Microsoft.Practices.Unity;
using Moq;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    public class SchedulerActorTests : TestKit
    {
        public SchedulerActorTests(ITestOutputHelper helper)
        {
            _container = new UnityContainer();
            var log = new XUnitAutoTestLoggerConfiguration(helper).CreateLogger();
            _container.RegisterInstance<ILogger>(log);
            var publisherMoq = new Mock<IPublisher>();
            _container.RegisterInstance(publisherMoq.Object);

            Sys.AddDependencyResolver(new UnityDependencyResolver(_container, Sys));
            var ext = Sys.InitSchedulingExtension(new InMemoryQuartzConfig(), log, new Mock<IPublisher>().Object, new Mock<ICommandExecutor>().Object);
            _scheduler = ext.SchedulingActor;
            _quartzScheduler = _container.Resolve<IScheduler>();
        }

        private const string Name = "test";
        private const string Group = "test";

        private readonly IActorRef _scheduler;
        private IScheduler _quartzScheduler;
        private readonly UnityContainer _container;

        private ExecutionOptions CreateOptions(double seconds,
                                                       TimeSpan? timeout = null,
                                                       Guid? id = null,
                                                       string checkField = null,
                                                       int? retryCount = null,
                                                       TimeSpan? repeatInterval = null)
        {
            return new ExecutionOptions(BusinessDateTime.UtcNow.AddSeconds(seconds),
                                                typeof(ScheduledCommandSuccessfullyProcessed),
                                                id ?? Guid.Empty,
                                                checkField,
                                                timeout);
        }

        [Fact]
        public void When_scheduler_is_restarted_Then_scheduled_jobs_still_get_executed()
        {
            var tasks = new[] {0.5, 1, 1.5, 2, 2.5};

            foreach (var task in tasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommandExecution(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }

            _quartzScheduler.Shutdown(false);
            _quartzScheduler = _container.Resolve<IScheduler>();

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.AssertInTime(() => ResultHolder.Contains(taskIds));
        }

        [Fact]
        public void When_some_of_scheduled_jobs_fail_System_still_executes_others()
        {
            var successTasks = new[] {0.5, 1.5, 2.5};
            var failTasks = new[] {1.0, 2.0};

            foreach (var task in successTasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testCommand = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommandExecution(testCommand, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }
            foreach (var failTask in failTasks)
            {
                var text = failTask.ToString(CultureInfo.InvariantCulture);
                var failTaskCommand = new FailCommand();
                _scheduler.Tell(new ScheduleCommandExecution(failTaskCommand,
                                                    new ScheduleKey(Guid.Empty, text, text),
                                                    CreateOptions(failTask)));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var failTaskIds = failTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.AssertInTime(() =>
                                  {
                                      ResultHolder.Contains(successTaskIds);
                                      Assert.True(failTaskIds.All(x => ResultHolder.Get(x) == null));
                                  },
                                  TimeSpan.FromSeconds(3));
        }

        [Fact]
        public void When_tasks_get_deleted_after_scheduling_System_will_not_execute_them()
        {
            var successTasks = new[] {0.5, 1.5, 2.5};
            var tasksToRemove = new[] {1.0, 2.0};

            foreach (var task in successTasks.Concat(tasksToRemove))
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommandExecution(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var tasksToRemoveTaskIds = tasksToRemove.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            foreach (var taskId in tasksToRemoveTaskIds)
                _scheduler.Tell(new Unschedule(new ScheduleKey(Guid.Empty, taskId, taskId)));

            Throttle.AssertInTime(() =>
                                  {
                                      ResultHolder.Contains(successTaskIds);
                                      Assert.True(tasksToRemoveTaskIds.All(x => ResultHolder.Get(x) == null));
                                  },
                                  TimeSpan.FromSeconds(4));
        }

        [Fact]
        public void When_there_are_several_scheduled_jobs_System_executes_all_of_them()
        {
            var tasks = new[] {0.5, 0.6, 0.7, 0.8, 1};

            foreach (var task in tasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommandExecution(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.AssertInTime(() => ResultHolder.Contains(taskIds));
        }
    }
}