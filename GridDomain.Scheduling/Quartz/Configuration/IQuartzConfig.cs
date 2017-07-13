using System.Collections.Specialized;
using GridDomain.Scheduling.Quartz.Retry;

namespace GridDomain.Scheduling.Quartz.Configuration
{
    //TODO: refactor to avoid only on of properties usage in implementation
    public interface IQuartzConfig
    {
        NameValueCollection Settings { get; }
        string Name { get; }

        IRetrySettings RetryOptions { get; set; }
    }
}