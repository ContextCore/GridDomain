using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace GridDomain.Tests.Scenarios {
    public class AnyIdStringAwareComparer : BaseTypeComparer
    {
        public AnyIdStringAwareComparer(RootComparer rootComparer) : base(rootComparer) {}

        public override bool IsTypeMatch(Type type1, Type type2)
        {
            return type1 == type2 && type1 == typeof(string);
        }

        public override void CompareType(CompareParms parms)
        {
            var guidA = (string) parms.Object1;
            var guidB = (string) parms.Object2;

            if (guidA == Any.GUID.ToString() || guidB == Any.GUID.ToString() || guidA == guidB)
                return;

            parms.Result.Differences.Add(new Difference
                                         {
                                             PropertyName = parms.BreadCrumb,
                                             Object1Value = guidA,
                                             Object2Value = guidB
                                         });
        }
    }
}