using System;
using System.Linq;

namespace GridDomain.Tests.Framework
{
    public class CannotCreateGenericType : Exception
    {
        public CannotCreateGenericType(Type type, Type[] typeConstraints)
            : base(
                $"Cannot create generic type {type} : Cannot find type to use as parameter with constraints "
                + $"{string.Join(",", typeConstraints.Select(t => t.Name))} "
                + $" Include assembly in AllAssemblies test property")
        {
            TypeConstraints = typeConstraints;
            Type = type;
        }

        public Type Type { get; }
        public Type[] TypeConstraints { get; }
    }
}