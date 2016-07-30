using System;
using System.Linq.Expressions;
using GridDomain.Common;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public static class ProjectionGroupExtensions
    {
        public static void Add<TMessage, THandler>(this ProjectionGroup group,
            Expression<Func<TMessage, Guid>> correlationPropertyExpression) where THandler : IHandler<TMessage> where TMessage : class
        {
            group.Add<TMessage,THandler>(MemberNameExtractor.GetName(correlationPropertyExpression));
        }
    }
}