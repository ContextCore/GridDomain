using GridDomain.Logging;
using Quartz;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public class LoggingSchedulerListener : ILoggingSchedulerListener
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();

        public void JobScheduled(ITrigger trigger)
        {
            _log.Info("Job {JobKey} scheduled for next execution {NextFireTime}", trigger.JobKey, trigger.GetNextFireTimeUtc());
        }

        public void JobUnscheduled(TriggerKey triggerKey)
        {
            _log.Info("Trigger {TriggerKey} unscheduled ", triggerKey);
        }

        public void TriggerFinalized(ITrigger trigger)
        {
            _log.Info("Trigger {TriggerKey}  for job {JobKey} finalized and won`t fire again", trigger.Key,trigger.JobKey );
        }

        public void TriggerPaused(TriggerKey triggerKey)
        {
            _log.Info("Trigger {TriggerKey} paused", triggerKey);
        }

        public void TriggersPaused(string triggerGroup)
        {
            _log.Info("Triggers in group {TriggerGroup} paused", triggerGroup);
        }

        public void TriggerResumed(TriggerKey triggerKey)
        {
            _log.Info("Trigger {TriggerKey} resumed", triggerKey);
        }

        public void TriggersResumed(string triggerGroup)
        {
            _log.Info("Triggers in group {TriggerGroup} resumed", triggerGroup);
        }

        public void JobAdded(IJobDetail jobDetail)
        {
            _log.Info("Job {JobKey} added", jobDetail.Key);
        }

        public void JobDeleted(JobKey jobKey)
        {
            _log.Info("Job {JobKey} deleted", jobKey);
        }

        public void JobPaused(JobKey jobKey)
        {
            _log.Info("Job {JobKey} paused", jobKey);
        }

        public void JobsPaused(string jobGroup)
        {
            _log.Info("Jobs in group {JobGroup} paused", jobGroup);
        }

        public void JobResumed(JobKey jobKey)
        {
            _log.Info("Job {JobKey} resumed", jobKey);
        }

        public void JobsResumed(string jobGroup)
        {
            _log.Info("Jobs in group {JobGroup} resumed", jobGroup);
        }

        public void SchedulerError(string msg, SchedulerException cause)
        {
            _log.Error(cause, "Scheduler error {message}",msg);
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