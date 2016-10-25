using GridDomain.Logging;
using Quartz;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public class LoggingSchedulerListener : ILoggingSchedulerListener
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();

        public void JobScheduled(ITrigger trigger)
        {
            _log.Trace("Job {JobKey} scheduled for next execution {NextFireTime}", trigger.JobKey, trigger.GetNextFireTimeUtc());
        }

        public void JobUnscheduled(TriggerKey triggerKey)
        {
            _log.Trace("Trigger {TriggerKey} unscheduled ", triggerKey);
        }

        public void TriggerFinalized(ITrigger trigger)
        {
            _log.Trace("Trigger {TriggerKey}  for job {JobKey} finalized and won`t fire again", trigger.Key,trigger.JobKey );
        }

        public void TriggerPaused(TriggerKey triggerKey)
        {
            _log.Trace("Trigger {TriggerKey} paused", triggerKey);
        }

        public void TriggersPaused(string triggerGroup)
        {
            _log.Trace("Triggers in group {TriggerGroup} paused", triggerGroup);
        }

        public void TriggerResumed(TriggerKey triggerKey)
        {
            _log.Trace("Trigger {TriggerKey} resumed", triggerKey);
        }

        public void TriggersResumed(string triggerGroup)
        {
            _log.Trace("Triggers in group {TriggerGroup} resumed", triggerGroup);
        }

        public void JobAdded(IJobDetail jobDetail)
        {
            _log.Trace("Job {JobKey} added", jobDetail.Key);
        }

        public void JobDeleted(JobKey jobKey)
        {
            _log.Trace("Job {JobKey} deleted", jobKey);
        }

        public void JobPaused(JobKey jobKey)
        {
            _log.Trace("Job {JobKey} paused", jobKey);
        }

        public void JobsPaused(string jobGroup)
        {
            _log.Trace("Jobs in group {JobGroup} paused", jobGroup);
        }

        public void JobResumed(JobKey jobKey)
        {
            _log.Trace("Job {JobKey} resumed", jobKey);
        }

        public void JobsResumed(string jobGroup)
        {
            _log.Trace("Jobs in group {JobGroup} resumed", jobGroup);
        }

        public void SchedulerError(string msg, SchedulerException cause)
        {
            _log.Error(cause, "Scheduler error {message}",msg);
        }

        public void SchedulerInStandbyMode()
        {
            _log.Trace("Scheduler goes to stand by mode");
        }

        public void SchedulerStarted()
        {
            _log.Trace("Scheduler started");
        }

        public void SchedulerStarting()
        {
            _log.Trace("Scheduler starting");
        }

        public void SchedulerShutdown()
        {
            _log.Trace("Scheduler shut down");
        }

        public void SchedulerShuttingdown()
        {
            _log.Trace("Scheduler shutting down");
        }

        public void SchedulingDataCleared()
        {
            _log.Trace("Scheduling data cleared");
        }
    }
}