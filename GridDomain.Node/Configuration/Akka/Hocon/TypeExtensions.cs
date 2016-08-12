using System;

namespace GridDomain.Node.Configuration.Akka.Hocon
{
    public static class TypeExtensions
    {
        public static string AssemblyQualifiedShortName(this Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }
    }
}