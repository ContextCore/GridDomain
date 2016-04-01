using System;
using System.Collections.Generic;
using System.Linq;

namespace GridDomain.Tests.Acceptance.Persistence
{
    public static class AutoFix
    {
        static Random rnd = new Random();
        public static T RandomElement<T>(this ICollection<T> collection)
        {
            int totalLength = collection.Count();
            int elemIndex = rnd.Next(0, totalLength);
            return collection.Skip(elemIndex - 1).FirstOrDefault();
        }

        public static DateTime RandomTime(DateTime from, DateTime till)
        {
            if (till > from) throw new ArgumentOutOfRangeException();

            var randomTime = till.AddDays(rnd.Next(0, (int)(@from - till).TotalDays));
            return randomTime;
        }
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}