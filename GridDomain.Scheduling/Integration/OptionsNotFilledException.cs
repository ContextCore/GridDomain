using System;

namespace GridDomain.Scheduling.Integration {
    internal class OptionsNotFilledException : Exception
    {
        public OptionsNotFilledException(string s) : base(s) {}
    }
}