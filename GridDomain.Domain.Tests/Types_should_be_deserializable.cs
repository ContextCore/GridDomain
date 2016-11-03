using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace GridDomain.Tests
{
    public class Types_should_be_deserializable
    {
        private readonly ObjectDeserializationChecker _checker = new ObjectDeserializationChecker();

        protected void CheckAllChildrenOf<T>(params Assembly[] assembly)
        {
            var allTypes =
                assembly.SelectMany(a => a.GetTypes())
                        .Where(t => typeof(T).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                        .Distinct();

            CheckAllChildrenOfAssembly<T>(allTypes.ToArray());
        }

        class RestoreResult
        {
            public string Difference;
            public Exception Exception;
            public Type Type;
        }

        protected void CheckAllChildrenOfAssembly<T>(params Type[] types)
        {
            var fixture = new Fixture();
            var failedTypes = new List<RestoreResult>();
            var okTypes = new List<RestoreResult>();

            foreach (var type in types)
            {
                try
                {
                    var createMethodInfo =
                        typeof(SpecimenFactory).GetMethod(nameof(SpecimenFactory.Create),
                            new[] {typeof(ISpecimenBuilder)}).MakeGenericMethod(type);
                    var obj = createMethodInfo.Invoke(null, new object[] {fixture});
                    string difference;

                    if (_checker.IsRestorable(obj, out difference))
                        okTypes.Add(new RestoreResult {Difference = difference, Type = type});
                    else
                        okTypes.Add(new RestoreResult {Type = type});
                }
                catch (Exception ex)
                {
                    failedTypes.Add(new RestoreResult {Exception = ex, Type = type});
                }
            }

            var sb = new StringBuilder();
            if (failedTypes.Count > 0)
            {
                AddFailedTypes(sb, failedTypes);
                AddOkTypes(sb, okTypes);
                Assert.Fail(sb.ToString());
            }
            AddOkTypes(sb, okTypes);
            Assert.Pass(sb.ToString());
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


            foreach (var res in failedTypes)
            {
                sb.AppendLine(res.Type.Name);
            }
        }

    }
}