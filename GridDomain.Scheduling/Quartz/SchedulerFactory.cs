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
        private readonly IJobFactory _jobFactory;
        private static readonly object _locker = new object();
        private IScheduler _current;
        bool _disposing = false;
        private readonly ILogger _log;
        private readonly IRetryStrategy _retryStrategy;

        public SchedulerFactory(
                    IQuartzConfig config,
                    IJobFactory jobFactory,
                    ILogger log, 
                    IRetryStrategy retryStrategy)
        {
            _log = log;
            _retryStrategy = retryStrategy;
            _config = config;
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
            _log.Information("Creating new scheduler instance");

            var properties = _config.Settings;

            properties["quartz.scheduler.instanceId"] = "AUTO";
            properties["quartz.threadPool.threadCount"] = "1";

            if (name != null)
            {
                properties["quartz.scheduler.instanceName"] = name;
            }

            var stdSchedulerFactory = new StdSchedulerFactory(properties);
            stdSchedulerFactory.Initialize();

            var scheduler = stdSchedulerFactory.GetScheduler();
            scheduler.JobFactory = _jobFactory;
            scheduler.ListenerManager.AddSchedulerListener(new LoggingSchedulerListener(_log));
            scheduler.ListenerManager.AddJobListener(new LoggingJobListener(_log), GroupMatcher<JobKey>.AnyGroup());
            scheduler.ListenerManager.AddJobListener(new RetryJobListener(_retryStrategy,_log), GroupMatcher<JobKey>.AnyGroup());

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


        public void Dispose()
        {
            if (!_disposing)
                 ((StdScheduler)_current)?.Dispose();
            _disposing = true;
        }
    }
}