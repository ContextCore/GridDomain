using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public static class CommandExpectBuilderExtensions
    {
        public static Task<IWaitResult> Execute(this IConditionCommandExecutor builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResult> Execute(this IConditionCommandExecutor builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }

        public static Task<IWaitResult> Execute(this IConditionCommandExecutor builder)
        {
            return builder.Execute(null, true);
        }

        public static Task<IWaitResult<T>> Execute<T>(this IConditionCommandExecutor<T> builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResult<T>> Execute<T>(this IConditionCommandExecutor<T> builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }

        public static Task<IWaitResult<T>> Execute<T>(this IConditionCommandExecutor<T> builder)
        {
            return builder.Execute(null, true);
        }
    }
}