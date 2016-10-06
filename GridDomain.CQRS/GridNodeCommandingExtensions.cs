using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public static class CommandingExtensions
    {
        public static Task<T> Execute<T>(this ICommandExecutor node, CommandPlan<T> data)
        {
            return node.Execute(data)
                       .ContinueWithSafeResultCast(result => (T)result);

        }
    }
}