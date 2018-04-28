using System;
using System.Collections.Generic;

namespace GridDomain.CQRS
 {
     /// <summary>
     /// Combines predicates about messages to create a complex condition (rule),
     /// that will be used for messages to decide about other actions
     /// </summary>
     /// <typeparam name="TBuilder"></typeparam>
     public interface IConditionBuilder<out TBuilder>
     {
         TBuilder And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
         TBuilder Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
         IReadOnlyCollection<Type> KnownMessageTypes { get; }
         bool Check(params object[] messages);
     }
     
     /// <summary>
     /// Create conditions to receiving messages, defining was all expecte messages arrived or not
     /// </summary>
     public interface IExpectedMessageBox<out TBuilder>
     {
         bool Receive(object message);
         bool AllExpectedMessagesReceived();
         IReadOnlyCollection<object> ReceivedMessages { get; }
     }
     
     public interface IConditionFactory<out T, out TBuilder>: IConditionBuilder<TBuilder>
     {
         T Create();
         IReadOnlyCollection<Type> RequiredMessageTypes { get; }
     }
     
     public interface IConditionFactory<out T>:IConditionFactory<T,IConditionFactory<T>>
     {
     }
 }