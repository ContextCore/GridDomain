using System;
 
 namespace GridDomain.CQRS
 {

     public interface IConditionBuilder<out TBuilder>
     {
         TBuilder And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
         TBuilder Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class; 
     }
     public interface IConditionFactory<out T, out TBuilder>: IConditionBuilder<TBuilder>
     {
         T Create();
     }
     
     public interface IConditionFactory<out T>:IConditionFactory<T,IConditionFactory<T>>
     {
     }
 }