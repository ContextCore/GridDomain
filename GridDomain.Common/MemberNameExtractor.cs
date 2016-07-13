using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GridDomain.Common
{
    public static class MemberNameExtractor
    {
        public static string GetName<T, U>(Expression<Func<T, U>> property)
        {
            MemberExpression memberExpression;

            if (property.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) property.Body;
                memberExpression = (MemberExpression) unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression) property.Body;
            }

            var memberInfo = memberExpression.Member as PropertyInfo;
            if(memberInfo == null)
                throw new ArgumentException("Cannot find property while extracting name from expressiom");
            return  memberInfo.Name;
        }
    }
}
