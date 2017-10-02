using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public class LoggingSchedulerListener : ISchedulerListener
    {
        private readonly ILogger _log;

        public LoggingSchedulerListener(ILogger log)
        {
            _log = log.ForContext("ListenerHash", GetHashCode());
            _log.Verbose("Created scheduler listener {ListenerHash}");
        }

        public Task JobScheduled(ITrigger trigger, CancellationToken token)
        {
            _log.Verbose("Job {JobKey} scheduled for execution {NextFireTime}", trigger.JobKey, trigger.StartTimeUtc);
            return Task.CompletedTask;
        }

        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken token)
        {
            _log.Verbose("Trigger {TriggerKey} unscheduled ", triggerKey);
            return Task.CompletedTask;
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken token)
        {
            _log.Verbose("Trigger {TriggerKey} for job {JobKey} finalized and won`t fire again", trigger.Key, trigger.JobKey);
            return Task.CompletedTask;
        }

        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken token)
        {
            _log.Verbose("Trigger {TriggerKey} paused", triggerKey);
            return Task.CompletedTask;
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken token)
        {
            _log.Verbose("Triggers in group {TriggerGroup} paused", triggerGroup);
            return Task.CompletedTask;
        }

        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken token)
        {
            _log.Verbose("Trigger {TriggerKey} resumed", triggerKey);
            return Task.CompletedTask;
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken token)
        {
            _log.Verbose("Triggers in group {TriggerGroup} resumed", triggerGroup);
            return Task.CompletedTask;
        }

        public Task JobAdded(IJobDetail jobDetail, CancellationToken token)
        {
            _log.Verbose("Job {JobKey} added", jobDetail.Key);
            return Task.CompletedTask;
        }

        public Task JobDeleted(JobKey jobKey, CancellationToken token)
        {
            _log.Verbose("Job {JobKey} deleted", jobKey);
            return Task.CompletedTask;
        }

        public Task JobPaused(JobKey jobKey, CancellationToken token)
        {
            _log.Verbose("Job {JobKey} paused", jobKey);
            return Task.CompletedTask;
        }

        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            _log.Verbose("Job {JobKey} interrupted", jobKey);
            return Task.CompletedTask;
        }

        public Task JobsPaused(string jobGroup, CancellationToken token)
        {
            _log.Verbose("Jobs in group {JobGroup} paused", jobGroup);
            return Task.CompletedTask;
        }

        public Task JobResumed(JobKey jobKey, CancellationToken token)
        {
            _log.Verbose("Job {JobKey} resumed", jobKey);
            return Task.CompletedTask;
        }

        public Task JobsResumed(string jobGroup, CancellationToken token)
        {
            _log.Verbose("Jobs in group {JobGroup} resumed", jobGroup);
            return Task.CompletedTask;
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken token)
        {
            _log.Error(cause, "Scheduler error {@message}", msg);
            return Task.CompletedTask;
        }

        public Task SchedulerInStandbyMode(CancellationToken token)
        {
            _log.Verbose("Scheduler goes to stand by mode");
            return Task.CompletedTask;
        }

        public Task SchedulerStarted(CancellationToken token)
        {
            _log.Verbose("Scheduler listener started at thread {Thread}.", Environment.CurrentManagedThreadId);
            return Task.CompletedTask;
        }

        public Task SchedulerStarting(CancellationToken token)
        {
            _log.Verbose("Scheduler listener {ListenerHash} starting");
            return Task.CompletedTask;
        }

        public Task SchedulerShutdown(CancellationToken token)
        {
            _log.Verbose("Scheduler listener {ListenerHash} shut down");
            return Task.CompletedTask;
        }

        public Task SchedulerShuttingdown(CancellationToken token)
        {
            _log.Verbose("Scheduler listener {ListenerHash} shutting down");
            return Task.CompletedTask;
        }

        public Task SchedulingDataCleared(CancellationToken token)
        {
            _log.Verbose("Scheduling data cleared.{hash} Thread {Thread}.", GetHashCode(), Environment.CurrentManagedThreadId);
            return Task.CompletedTask;
        }
    }
}