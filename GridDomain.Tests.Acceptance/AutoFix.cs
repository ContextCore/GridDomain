using System;
using System.Collections.Generic;
using System.Linq;

namespace GridDomain.Tests.Acceptance
{
    public static class AutoFix
    {
        private static readonly Random Rnd = new Random();

        public static T RandomElement<T>(this ICollection<T> collection)
        {
            var totalLength = collection.Count();
            var elemIndex = Rnd.Next(0, totalLength);
            return collection.Skip(elemIndex - 1).FirstOrDefault();
        }

        public static DateTime RandomTime(DateTime from, DateTime till)
        {
            if (till > from) throw new ArgumentOutOfRangeException();

            var randomTime = till.AddDays(Rnd.Next(0, (int) (@from - till).TotalDays));
            return randomTime;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rnd.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}