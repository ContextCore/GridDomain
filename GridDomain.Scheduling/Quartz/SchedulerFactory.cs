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
    public class QuartzSchedulerFactory : StdSchedulerFactory

    {
        private readonly IJobFactory _jobFactory;
        private readonly ILogger _log;
        private readonly IRetryStrategy _retryStrategy;

        public QuartzSchedulerFactory(
                    IJobFactory jobFactory,
                    ILogger log,
                    IRetryStrategy retryStrategy,
                    IQuartzConfig config):base(config.Settings)
        {
            _log = log;
            _retryStrategy = retryStrategy;
            _jobFactory = jobFactory;
        }

        public override void Initialize(NameValueCollection props)
        {
            base.Initialize(props);
        }

        public override IScheduler GetScheduler(string schedName)
        {
            var scheduler = base.GetScheduler(schedName);
            scheduler.JobFactory = _jobFactory;
            scheduler.ListenerManager.AddSchedulerListener(new LoggingSchedulerListener(_log));
            scheduler.ListenerManager.AddJobListener(new LoggingJobListener(_log), GroupMatcher<JobKey>.AnyGroup());
            scheduler.ListenerManager.AddJobListener(new RetryJobListener(_retryStrategy, _log), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();
            return scheduler;
        }
    }


    //    public class SchedulerFactory : ISchedulerFactory, IDisposable
    //{
    //    private readonly IQuartzConfig _config;
    //    private readonly IJobFactory _jobFactory;
    //    private static readonly object _locker = new object();
    //    private IScheduler _current;
    //    bool _disposing = false;
    //    private readonly ILogger _log;
    //    private readonly IRetryStrategy _retryStrategy;

    //    public SchedulerFactory(
    //                IQuartzConfig config,
    //                IJobFactory jobFactory,
    //                ILogger log, 
    //                IRetryStrategy retryStrategy)
    //    {
    //        _log = log;
    //        _retryStrategy = retryStrategy;
    //        _config = config;
    //        _jobFactory = jobFactory;
    //    }

    //    public IScheduler GetScheduler()
    //    {
    //        return GetScheduler(null);
    //    }

    //    public IScheduler GetScheduler(string schedName)
    //    {
    //        lock (_locker)
    //        {
    //            if (_current == null || _current.IsShutdown)
    //            {
    //                _current = Create(schedName);
    //            }
    //        }
    //        return _current;
    //    }

    //    public ICollection<IScheduler> AllSchedulers => new[] { _current };

    //    private IScheduler Create(string name)
    //    {
    //        _log.Information("Creating new scheduler instance");

    //        var stdSchedulerFactory = new StdSchedulerFactory();
    //        stdSchedulerFactory.Initialize(_config.Settings);

    //        var scheduler = name != null ? stdSchedulerFactory.GetScheduler(name) : stdSchedulerFactory.GetScheduler();

    //        scheduler.JobFactory = _jobFactory;
    //        scheduler.ListenerManager.AddSchedulerListener(new LoggingSchedulerListener(_log));
    //        scheduler.ListenerManager.AddJobListener(new LoggingJobListener(_log), GroupMatcher<JobKey>.AnyGroup());
    //        scheduler.ListenerManager.AddJobListener(new RetryJobListener(_retryStrategy,_log), GroupMatcher<JobKey>.AnyGroup());
    //        scheduler.Start();
    //        return scheduler;
    //    }


    //    public void Dispose()
    //    {
    //        if (!_disposing)
    //             ((StdScheduler)_current)?.Dispose();
    //        _disposing = true;
    //    }
    //}
}