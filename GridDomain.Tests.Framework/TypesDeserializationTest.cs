using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xunit;

namespace GridDomain.Tests.Framework
{
    public abstract class TypesDeserializationTest
    {
        private readonly HashSet<Type> _excludes;
        protected ObjectDeserializationChecker Checker = new ObjectDeserializationChecker();
        protected Fixture Fixture;

        protected TypesDeserializationTest()
        {
            TypesCache =
                AllAssemblies.SelectMany(a => a.GetTypes())
                             .SelectMany(t => t.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic).Union(new[] {t}))
                             .Where(t => t != null)
                             .Distinct()
                             .ToArray();

            _excludes = new HashSet<Type>(ExcludeTypes);

            Fixture = new Fixture();
        }

        protected abstract Assembly[] AllAssemblies { get; }
        protected virtual IEnumerable<Type> ExcludeTypes { get; } = new Type[] {};
        protected Type[] TypesCache { get; }

        protected void CheckAllChildrenOf<T>(params Assembly[] assembly)
        {
            var allTypes =
                assembly.SelectMany(a => a.GetTypes())
                        .Where(
                               t =>
                                   typeof(T).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsInterface
                                   && t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any())
                        .Distinct();

            CheckAll<T>(allTypes.ToArray());
        }

        protected void CheckAll<T>(params Type[] types)
        {
            var objects = new List<object>();
            var results = new List<RestoreResult>();
            foreach (var type in types.Where(t => !_excludes.Contains(t)))
                try
                {
                    var constructedType = type;

                    var createMethodInfo =
                        typeof(SpecimenFactory).GetMethod(nameof(SpecimenFactory.Create), new[] {typeof(ISpecimenBuilder)})
                                               .MakeGenericMethod(constructedType);


                    var obj = createMethodInfo.Invoke(null, new object[] {Fixture});
                    objects.Add(obj);
                }
                catch (Exception ex)
                {
                    results.Add(RestoreResult.Error(type,ex));
                }

            results.AddRange(RestoreAll(objects));

            CheckResults(results.ToArray());
        }

        protected static void CheckResults(params RestoreResult[] results)
        {
            var sb = new StringBuilder();


            if (results.Any(r => !r.IsOk))
            {
                AddFailedTypes(sb, results.Where(r => !r.IsOk));
                AddOkTypes(sb, results.Where(r => r.IsOk));
                Assert.True(false, sb.ToString());
            }

            AddOkTypes(sb, results.Where(r => r.IsOk));
            Assert.True(true, sb.ToString());
        }

        protected RestoreResult[] RestoreAll(params object[] objects)
        {
            var restoreResults = new List<RestoreResult>();
            foreach (var obj in objects)
                try
                {
                    restoreResults.Add(Checker.IsRestorable(obj, out string difference) ?
                        RestoreResult.Ok(obj.GetType()) : 
                        RestoreResult.Diff(obj.GetType(), difference));
                }
                catch (Exception ex)
                {
                    restoreResults.Add(RestoreResult.Error(obj.GetType(), ex));
                }

            return restoreResults.ToArray();
        }

        private static void AddFailedTypes(StringBuilder sb, IEnumerable<RestoreResult> failedTypes)
        {
            sb.AppendLine("Cannot restore types:");

            foreach (var res in failedTypes)
            {
                sb.AppendLine();
                sb.AppendLine("-------------------------------------------------");
                sb.AppendLine();
                sb.AppendLine($"Type: {res.Type}");
                sb.AppendLine();

                if (!string.IsNullOrEmpty(res.Difference))
                {
                    sb.AppendLine("Type was restored, buy with difference:");
                    sb.AppendLine(res.Difference);
                }
                if (res.Exception != null)
                {
                    sb.AppendLine("Type was not restored, Exception:");
                    sb.AppendLine(res.Exception.ToString());
                }
            }
        }

        private static void AddOkTypes(StringBuilder sb, IEnumerable<RestoreResult> failedTypes)
        {
            sb.AppendLine();
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine();
            sb.AppendLine("Successfully restored:");
            sb.AppendLine();


            foreach (var res in failedTypes)
                sb.AppendLine(res.Type.Name);
        }

        protected class RestoreResult
        {
            public string Difference { get; private set; }
            public Exception Exception { get; private set; }
            public Type Type { get; private set; }

            public bool IsOk  => Exception == null && Difference == null;

            public static RestoreResult Ok(Type t)
            {
                return new RestoreResult(){Type = t};
            }
            public static RestoreResult Diff(Type t, string difference)
            {
                return new RestoreResult() { Type = t, Difference = difference };
            }
            public static RestoreResult Error(Type t, Exception ex)
            {
                return new RestoreResult() {Exception = ex, Type = t};
            }
        }
    }
}