using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.Quartz.Retry;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Quartz.Util;
using Serilog;

namespace GridDomain.Scheduling.Quartz
{
    public class SchedulerFactory : ISchedulerFactory, IDisposable
    {
        private readonly IQuartzConfig _config;
        private readonly ILoggingSchedulerListener _loggingSchedulerListener;
        private readonly ILoggingJobListener _loggingJobListener;
        private readonly IJobFactory _jobFactory;
        private static readonly object _locker = new object();
        private IScheduler _current;
        public SchedulerFactory(
            IQuartzConfig config,
            ILoggingSchedulerListener loggingSchedulerListener,
            ILoggingJobListener loggingJobListener,
            IJobFactory jobFactory,
            IRetrySettings retrySettings,
            ILogger log
            )
        {
            _log = log;
            _retrySettings = retrySettings;
            _config = config;
            _loggingSchedulerListener = loggingSchedulerListener;
            _loggingJobListener = loggingJobListener;
            _jobFactory = jobFactory;
        }

        public IScheduler GetScheduler()
        {
            return GetScheduler(null);
        }

        public IScheduler GetScheduler(string schedName)
        {
            lock (_locker)
            {
                if (_current == null || _current.IsShutdown)
                {
                    _current = Create(schedName);
                }
            }
            return _current;
        }

        public ICollection<IScheduler> AllSchedulers => new[] { _current };

        private IScheduler Create(string name)
        {
            var properties = _config.Settings;

            properties["quartz.scheduler.instanceId"] = "AUTO";
            if (name != null)
            {
                properties["quartz.scheduler.instanceName"] = name;
            }

            var stdSchedulerFactory = new StdSchedulerFactory(properties);

            stdSchedulerFactory.Initialize();
            var scheduler = stdSchedulerFactory.GetScheduler();
            scheduler.JobFactory = _jobFactory;
            scheduler.ListenerManager.AddSchedulerListener(_loggingSchedulerListener);
            scheduler.ListenerManager.AddJobListener(_loggingJobListener);

            IRetryStrategy retryStrategy = new ExponentialBackoffRetryStrategy(_retrySettings, _log);
            IJobListener retryListener = new RetryJobListener(retryStrategy, _log);
            scheduler.ListenerManager.AddJobListener(retryListener, GroupMatcher<JobKey>.AnyGroup());

            try
            {
                scheduler.Start();
            }
            catch (SchedulerException)
            {
                lock (_locker)
                {
                    _current = null;
                    return GetScheduler(name);
                }
            }
            
            return scheduler;
        }
        bool _disposing = false;
        private readonly IRetrySettings _retrySettings;
        private readonly ILogger _log;

        public void Dispose()
        {
            if (!_disposing)
                 ((StdScheduler)_current)?.Dispose();
            _disposing = true;
        }
    }
}