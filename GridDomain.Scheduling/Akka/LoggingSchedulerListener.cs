using NLog;
using Quartz;
using LogManager = NLog.LogManager;

namespace GridDomain.Scheduling.Akka
{
    public class LoggingSchedulerListener : ILoggingSchedulerListener
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger(typeof(LoggingSchedulerListener));

        public void JobScheduled(ITrigger trigger)
        {
            _log.Info($"Job {trigger.JobKey} scheduled for next execution {trigger.GetNextFireTimeUtc()}");
        }

        public void JobUnscheduled(TriggerKey triggerKey)
        {
            _log.Info($"Trigger {triggerKey} unscheduled ");
        }

        public void TriggerFinalized(ITrigger trigger)
        {
            _log.Info($"Trigger {trigger.Key} for job {trigger.JobKey} finalized and won`t fire again");
        }

        public void TriggerPaused(TriggerKey triggerKey)
        {
            _log.Info($"Trigger {triggerKey} paused");
        }

        public void TriggersPaused(string triggerGroup)
        {
            _log.Info($"Triggers in group {triggerGroup} paused");
        }

        public void TriggerResumed(TriggerKey triggerKey)
        {
            _log.Info($"Trigger {triggerKey} resumed");
        }

        public void TriggersResumed(string triggerGroup)
        {
            _log.Info($"Triggers in group {triggerGroup} resumed");
        }

        public void JobAdded(IJobDetail jobDetail)
        {
            _log.Info($"Job {jobDetail.Key} added");
        }

        public void JobDeleted(JobKey jobKey)
        {
            _log.Info($"Job {jobKey} deleted");
        }

        public void JobPaused(JobKey jobKey)
        {
            _log.Info($"Job {jobKey} paused");
        }

        public void JobsPaused(string jobGroup)
        {
            _log.Info($"Jobs in group {jobGroup} paused");
        }

        public void JobResumed(JobKey jobKey)
        {
            _log.Info($"Job {jobKey} resumed");
        }

        public void JobsResumed(string jobGroup)
        {
            _log.Info($"Jobs in group {jobGroup} resumed");
        }

        public void SchedulerError(string msg, SchedulerException cause)
        {
            _log.Error(cause, $"Scheduler error {msg}");
        }

        public void SchedulerInStandbyMode()
        {
            _log.Info("Scheduler goes to stand by mode");
        }

        public void SchedulerStarted()
        {
            _log.Info("Scheduler started");
        }

        public void SchedulerStarting()
        {
            _log.Info("Scheduler starting");
        }

        public void SchedulerShutdown()
        {
            _log.Info("Scheduler shut down");
        }

        public void SchedulerShuttingdown()
        {
            _log.Info("Scheduler shutting down");
        }

        public void SchedulingDataCleared()
        {
            _log.Info("Scheduling data cleared");
        }
    }
}