using System;
using System.Linq.Expressions;

namespace GridDomain.Common
{
    public static class Condition
    {
        public static void NotNull<T>(Expression<Func<T>> expr)
        {
            var name = MemberNameExtractor.GetName(expr);
            if(expr.Compile().Invoke() == null)
                throw new ArgumentNullException(name);
        }
    }
}