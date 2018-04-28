using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public static class CommandExpectBuilderExtensions
    {
        public static Task<IWaitResult> Execute(this ICommandEventsFilter builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResult> Execute(this ICommandEventsFilter builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }

        public static Task<IWaitResult> Execute(this ICommandEventsFilter builder)
        {
            return builder.Execute(null, true);
        }

        public static Task<IWaitResult<T>> Execute<T>(this ICommandEventsFilter<T> builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResult<T>> Execute<T>(this ICommandEventsFilter<T> builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }

        public static Task<IWaitResult<T>> Execute<T>(this ICommandEventsFilter<T> builder)
        {
            return builder.Execute(null, true);
        }
    }
}