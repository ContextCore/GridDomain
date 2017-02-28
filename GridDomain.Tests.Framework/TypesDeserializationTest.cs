using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            var failedTypes = new List<RestoreResult>();
            var okTypes = new List<RestoreResult>();

            foreach (var type in types.Where(t => !_excludes.Contains(t)))
            {
                try
                {
                    var constructedType = type;

                    var createMethodInfo =
                        typeof(SpecimenFactory).GetMethod(nameof(SpecimenFactory.Create), new[] {typeof(ISpecimenBuilder)})
                                               .MakeGenericMethod(constructedType);


                    var obj = createMethodInfo.Invoke(null, new object[] {Fixture});
                    string difference;

                    if (Checker.IsRestorable(obj, out difference)) okTypes.Add(new RestoreResult {Type = constructedType});
                    else failedTypes.Add(new RestoreResult {Difference = difference, Type = constructedType});
                }
                catch (Exception ex) {
                    failedTypes.Add(new RestoreResult {Exception = ex, Type = type});
                }
            }

            var sb = new StringBuilder();
            if (failedTypes.Count > 0)
            {
                AddFailedTypes(sb, failedTypes);
                AddOkTypes(sb, okTypes);
                Assert.True(false, sb.ToString());
            }
            AddOkTypes(sb, okTypes);
            Assert.True(true, sb.ToString());
        }

        private static void AddFailedTypes(StringBuilder sb, List<RestoreResult> failedTypes)
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

        private static void AddOkTypes(StringBuilder sb, List<RestoreResult> failedTypes)
        {
            sb.AppendLine();
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine();
            sb.AppendLine("Successfully restored:");
            sb.AppendLine();


            foreach (var res in failedTypes) { sb.AppendLine(res.Type.Name); }
        }

        private class RestoreResult
        {
            public string Difference;
            public Exception Exception;
            public Type Type;
        }
    }
}