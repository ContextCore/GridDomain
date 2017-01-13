using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public static class CommandExpectBuilderExtensions
    {
        public static Task<IWaitResults> Execute(this ICommandExpectBuilder builder, TimeSpan timeout)
        {
            return builder.Execute(timeout, true);
        }

        public static Task<IWaitResults> Execute(this ICommandExpectBuilder builder, bool failOnAnyFault)
        {
            return builder.Execute(null, failOnAnyFault);
        }
        public static Task<IWaitResults> Execute(this ICommandExpectBuilder builder)
        {
            return builder.Execute(null, true);
        }
    }
}