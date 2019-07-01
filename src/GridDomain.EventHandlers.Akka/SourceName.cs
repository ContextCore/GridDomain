using System;
using System.Linq;
using GridDomain.Common;

namespace GridDomain.EventHandlers.Akka
{
    public static class SourceName
    {
        public static string Get<TMessage>(this Type handlerType)
        {
            return Get(handlerType, typeof(TMessage));
        }
        
        public static string Get<TMessageA,TMessageB>(this Type handlerType)
        {
            return Get(handlerType, typeof(TMessageA), typeof(TMessageB));
        }

        
        public static string Get<THandler,TMessage>()
        {
            return Get(typeof(THandler), typeof(TMessage));
        }
        public static string Get<THandler,TMessageA,TMessageB>()
        {
            return Get(typeof(THandler), typeof(TMessageA),typeof(TMessageB));
        }
        
        public static string Get(params Type[] types)
        {
            return string.Join("_", types.Select(t => TypeNameExtensions.BeautyName(t)));
        }
    }
    
}