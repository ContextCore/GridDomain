using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public static class GridNodeCommandingExtensions
    {
        public static Task<T> Execute<T>(this IGridDomainNode node, CommandPlan<T> data)
        {
            return node.Execute(data.Command, data.ExpectedMessages, data.Timeout)
                       .ContinueWithSafeResultCast(result => (T)result);

        }

        public static T ExecuteSync<T>(this IGridDomainNode node, CommandPlan<T> data)
        {
            var task = Execute<T>(node, data);

            try
            {
                if (!task.Wait(data.Timeout))
                    throw new TimeoutException("Command execution timed out");
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.UnwrapSingle()).Throw();
            }

            return task.Result;
        }
    }
}