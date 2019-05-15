using System;
using System.Reflection;

namespace GridDomain.Node.Akka.Configuration.Hocon
{
    public static class TypeExtensions
    {
        public static string AssemblyQualifiedShortName(this Type type)
        {
            return type.FullName + ", " + type.GetTypeInfo().Assembly.GetName().Name;
        }
    }
}