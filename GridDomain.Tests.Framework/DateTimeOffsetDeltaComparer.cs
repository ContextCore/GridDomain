using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace GridDomain.Tests.Framework
{
    public class DateTimeOffsetDeltaComparer : BaseTypeComparer
    {
        private readonly TimeSpan _delta;

        public DateTimeOffsetDeltaComparer(RootComparer rootComparer, TimeSpan? delta = null) : base(rootComparer)
        {
            _delta = delta ?? TimeSpan.FromSeconds(1);
        }

        public override bool IsTypeMatch(Type type1, Type type2)
        {
            return type1 == typeof(DateTimeOffset);
        }

        public override void CompareType(CompareParms parms)
        {
            var st1 = (DateTimeOffset) parms.Object1;
            var st2 = (DateTimeOffset) parms.Object2;

            if (TimeSpan.FromTicks(Math.Abs(st1.Ticks - st2.Ticks)) > _delta)
                AddDifference(parms);
        }
    }
}