using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public static class CommandFaultFactory
    {
        public static object CreateGenericFor(ICommand command, Exception ex)
        {
            var type = command.GetType();
            var faultType = typeof(CommandFault<>).MakeGenericType(type);
            var fault = faultType.GetConstructor(new [] { type, typeof(Exception), typeof(DateTime)})
                                 .Invoke(new object[] { command, ex, BusinessDateTime.UtcNow });
            return fault;
        }
    }
}