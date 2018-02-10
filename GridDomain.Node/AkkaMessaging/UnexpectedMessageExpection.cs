using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class UnexpectedMessageExpection : Exception
    {
        public UnexpectedMessageExpection(string description):base(description)
        {
            
        }
    }
}