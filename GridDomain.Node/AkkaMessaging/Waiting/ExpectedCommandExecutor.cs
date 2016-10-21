using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class ExpectedCommandExecutor : IExpectedCommandExecutor
    {
        public ICommandExecutor Executor { get; }
        public Task<IWaitResults> Awaiter { get; }

        public ExpectedCommandExecutor(ICommandExecutor executor, Task<IWaitResults> awaiter )
        {
            Awaiter = awaiter;
            Executor = executor;
        }

        public Task<IWaitResults> Execute(ICommand command, bool failOnFaults = true)
        {
            Executor.Execute(command);
            return Awaiter.ContinueWith(t =>
            {
                if (!failOnFaults) return t.Result;

                var faults = t.Result.All.OfType<IFault>().ToArray();
                if (faults.Any())
                    throw new AggregateException(faults.Select(f => f.Exception));
                return t.Result;
            });
        }
    }
}