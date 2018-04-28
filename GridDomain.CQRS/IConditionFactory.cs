using System;
using System.Collections.Generic;

namespace GridDomain.CQRS
 {
     /// <summary>
     /// Combines predicates about messages to create a complex condition (rule),
     /// that will be used for messages to decide about other actions
     /// </summary>
     /// <typeparam name="TBuilder"></typeparam>
     public interface IMessageFilter<out TBuilder>
     {
         TBuilder And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
         TBuilder Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
         IReadOnlyCollection<Type> KnownMessageTypes { get; }
         bool Check(params object[] messages);
     }
     

 }