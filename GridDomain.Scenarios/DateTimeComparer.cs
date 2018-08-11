using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace GridDomain.Tests.Scenarios
{
    public class DateTimeComparer : BaseTypeComparer
    {
        public DateTimeComparer(RootComparer rootComparer) : base(rootComparer) {}

        public override bool IsTypeMatch(Type type1, Type type2)
        {
            return type1 == type2 && type1 == typeof(DateTime);
        }

        public override void CompareType(CompareParms parms)
        {
            var guidA = (DateTime) parms.Object1;
            var guidB = (DateTime) parms.Object2;

            if (guidA == Any.DateTime || guidB == Any.DateTime || guidA == guidB)
                return;

            parms.Result.Differences.Add(new Difference
                                         {
                                             PropertyName = parms.BreadCrumb,
                                             Object1Value = guidA.ToString(),
                                             Object2Value = guidB.ToString()
                                         });
        }
    }
}