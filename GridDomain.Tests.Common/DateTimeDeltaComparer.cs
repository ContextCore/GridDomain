using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace GridDomain.Tests.Common
{
    public class DateTimeDeltaComparer : BaseTypeComparer
    {
        private readonly TimeSpan _delta;

        public DateTimeDeltaComparer(RootComparer rootComparer, TimeSpan? delta = null) : base(rootComparer)
        {
            _delta = delta ?? TimeSpan.FromSeconds(1);
        }

        public override bool IsTypeMatch(Type type1, Type type2)
        {
            return type1 == typeof(DateTime);
        }

        public override void CompareType(CompareParms parms)
        {
            var st1 = (DateTime) parms.Object1;
            var st2 = (DateTime) parms.Object2;

            if (TimeSpan.FromTicks(Math.Abs(st1.Ticks - st2.Ticks)) > _delta)
                AddDifference(parms);
        }
    }
}