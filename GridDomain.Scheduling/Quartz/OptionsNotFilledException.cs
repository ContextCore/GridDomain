using System;

namespace GridDomain.Scheduling.Quartz {
    internal class OptionsNotFilledException : Exception
    {
        public OptionsNotFilledException(string s) : base(s) {}
    }
}