using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GridDomain.Common
{
    public static class MemberNameExtractor
    {
        public static string GetName<T>(Expression<Func<T>> property)
        {
            MemberExpression memberExpression;

            var body = property.Body as UnaryExpression;
            if (body != null)
            {
                var unaryExpression = body;
                memberExpression = (MemberExpression) unaryExpression.Operand;
                return memberExpression.Member.Name;
            }

            memberExpression = property.Body as MemberExpression;
            if (memberExpression != null)
            {
                var memberInfo = memberExpression.Member as PropertyInfo;
                if (memberInfo != null) return memberInfo.Name;
                var fieldIngo = memberExpression.Member as FieldInfo;

                if (fieldIngo != null) return fieldIngo.Name;
            }

            var methodCallExpression = property.Body as MethodCallExpression;
            if (methodCallExpression != null) { return methodCallExpression.Method.Name; }

            throw new ArgumentException("Cannot extract name from expression");
        }

        public static string GetName<T, U>(Expression<Func<T, U>> property)
        {
            if (property == null) return null;

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
            if (memberInfo == null) throw new ArgumentException("Cannot find property while extracting name from expressiom");
            return memberInfo.Name;
        }
    }
}