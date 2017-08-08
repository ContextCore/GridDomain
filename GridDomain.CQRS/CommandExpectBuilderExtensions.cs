using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public static class CommandExpectBuilderExtensions
    {
        public static Task<IWaitResult> Execute(this ICommandConditionBuilder builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResult> Execute(this ICommandConditionBuilder builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }

        public static Task<IWaitResult> Execute(this ICommandConditionBuilder builder)
        {
            return builder.Execute(null, true);
        }

        public static Task<IWaitResult<T>> Execute<T>(this ICommandConditionBuilder<T> builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResult<T>> Execute<T>(this ICommandConditionBuilder<T> builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }

        public static Task<IWaitResult<T>> Execute<T>(this ICommandConditionBuilder<T> builder)
        {
            return builder.Execute(null, true);
        }
    }
}