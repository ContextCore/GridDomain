using System;
using System.Linq;

namespace GridDomain.Node.AkkaMessaging
{
    public static class TypeNameExtensions
    {
        /// <summary>
        /// Returns human-readable representation of generic name using only basic valid characters
        /// Will substitute all ` to _ for generic types
        /// </summary>
        /// <param name="type">type to get beauty name</param>
        /// <returns></returns>
        public static string BeautyName(this Type type)
        {
            if(!type.IsGenericType) 
                return type.Name;

            var typeName = type.Name.Split('`')[0];

            var parameters = string.Join("_",type.GetGenericArguments().Select(t=>t.BeautyName()));

            return $"{typeName}_{parameters}";
        }   
    }
}