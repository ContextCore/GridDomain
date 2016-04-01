using System.Linq;
using GridDomain.Logging;
using NEventStore;
using NLog;

namespace GridDomain.Tests.Acceptance.Persistence.CommandFlowProcessing
{
    public class LoggingPipelineHook : IPipelineHook
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public void Dispose()
        {
        }

        public ICommit Select(ICommit committed)
        {
            return committed;
        }

        public bool PreCommit(CommitAttempt attempt)
        {
            return true;
        }

        public void PostCommit(ICommit committed)
        {
            foreach (var msg in committed.Events.Select(e => e.Body))
                log.Trace($"событие {msg.GetType().Name} сохранено: " + msg.ToPropsString());
        }

        public void OnPurge(string bucketId)
        {
        }

        public void OnDeleteStream(string bucketId, string streamId)
        {
        }
    }
}