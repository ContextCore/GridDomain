using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public static class CommandExpectBuilderExtensions
    {
        public static Task<IWaitResults> Execute(this ICommandConditionBuilder builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResults> Execute(this ICommandConditionBuilder builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }

        public static Task<IWaitResults> Execute(this ICommandConditionBuilder builder)
        {
            return builder.Execute(null, true);
        }
    }
}