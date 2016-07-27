using System;
using System.Collections.Generic;
using System.Linq;

namespace GridDomain.Logging
{
    public static class TypesForScalarDescruptionHolder
    {
        private static readonly HashSet<Type> TypesToDesctructAsScalars = new HashSet<Type>();

        public static void Add(params Type[] types)
        {
            foreach (var type in types)
            {
                TypesToDesctructAsScalars.Add(type);
            }
        }

        public static Type[] Types => TypesToDesctructAsScalars.ToArray();
    }
}