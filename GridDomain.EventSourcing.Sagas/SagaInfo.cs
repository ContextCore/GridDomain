using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public static class SagaInfo<TSaga>
    {
        public static IReadOnlyCollection<Type> KnownMessages()
        {
            var allInterfaces = typeof(TSaga).GetInterfaces();

            var handlerInterfaces =
                allInterfaces.Where(i => i.IsGenericType &&
                                         i.GetGenericTypeDefinition() == typeof(IHandler<>))
                    .ToArray();

            return handlerInterfaces.SelectMany(s => s.GetGenericArguments()).ToArray();
        }
    }
}