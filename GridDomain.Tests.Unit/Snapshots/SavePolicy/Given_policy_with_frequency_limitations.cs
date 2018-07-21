using System;
using System.Threading.Tasks;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.Node;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Snapshots.SavePolicy
{
    public class Given_policy_with_frequency_limitations
    {
        private readonly Logger _logger;

        class LoggingPolicy : ISnapshotsSavePolicy
        {
            class LoggingTracker : IOperationTracker<long>
            {
                private readonly IOperationTracker<long> _operationTrackerImplementation;
                private readonly ILogger _logger;

                public LoggingTracker(ILogger log, IOperationTracker<long> operationTrackerImplementation)
                {
                    _logger = log;
                    _operationTrackerImplementation = operationTrackerImplementation;
                }

                public int InProgress => _operationTrackerImplementation.InProgress;

                public void Start(long criteria)
                {
                    _logger.Debug("Calling start {criteria}", criteria);
                    _operationTrackerImplementation.Start(criteria);
                }

                public void Complete(long instance)
                {
                    _logger.Debug("Calling complete {instance}", instance);
                    _operationTrackerImplementation.Complete(instance);
                }

                public void Fail(long instance)
                {
                    _logger.Debug("Calling Fail {instance}", instance);
                    _operationTrackerImplementation.Fail(instance);
                }
            }

            private readonly ISnapshotsSavePolicy _policy;
            private readonly LoggingTracker _loggingTracker;
            private readonly ILogger _logger;

            public LoggingPolicy(ILogger log, ISnapshotsSavePolicy policy)
            {
                _logger = log;
                _policy = policy;
                _loggingTracker = new LoggingTracker(log, _policy.Tracking);
            }

            public bool ShouldSave(long snapshotSequenceNr)
            {
                _logger.Debug("Calling should save {num}", snapshotSequenceNr);
                return _policy.ShouldSave(snapshotSequenceNr);
            }

            public IOperationTracker<long> Tracking => _loggingTracker;
        }

        public Given_policy_with_frequency_limitations(ITestOutputHelper helper)
        {
            _logger = new LoggerConfiguration().Default(LogEventLevel.Verbose)
                                               .WriteToFile(LogEventLevel.Verbose, nameof(When_saved_too_frequently_with_confirmation_Then_should_save_after_time_passed))
                                               .XUnit(LogEventLevel.Verbose,helper)
                                               .CreateLogger();
        }
        [Fact]
        public async Task When_saved_too_frequently_with_confirmation_Then_should_save_after_time_passed()
        {
            var maxSaveFrequency = TimeSpan.FromMilliseconds(50);
            var policy = new LoggingPolicy(_logger,new SnapshotsSavePolicy(1, maxSaveFrequency));
            Assert.True(policy.ShouldSave(1));
            policy.Tracking.Start(1);
            policy.Tracking.Complete(1);
            Assert.False(policy.ShouldSave(1));
            Assert.False(policy.ShouldSave(2));
            await Task.Delay(maxSaveFrequency);
            Assert.True(policy.ShouldSave(2));
        }

        [Fact]
        public async Task When_saved_too_frequently_and_persist_take_too_much_time_Then_should_save_before_confirmation()
        {
            var maxSaveFrequency = TimeSpan.FromMilliseconds(50);
            var policy = new LoggingPolicy(_logger,new SnapshotsSavePolicy(1, maxSaveFrequency));
            Assert.True(policy.ShouldSave(1));
            policy.Tracking.Start(1);
            Assert.False(policy.ShouldSave(1), "saving seq 1");
            Assert.False(policy.ShouldSave(2), "saving seq 2");
            await Task.Delay(maxSaveFrequency);
            Assert.True(policy.ShouldSave(2), "saving seq 2 after waiting");
        }

        [Fact]
        public void When_persist_fails_Then_should_not_count_it_as_save_attempt_and_reset_time_limitation()
        {
            var policy = new SnapshotsSavePolicy(1, TimeSpan.FromSeconds(1));
            Assert.True(policy.ShouldSave(1));
            policy.Tracking.Start(1);
            policy.Tracking.Fail(1);
            Assert.True(policy.ShouldSave(1));
            Assert.True(policy.ShouldSave(2));
        }
    }
}