using System;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders
{
    public class MessageHandleException : Exception
    {
        public readonly BalloonTitleChanged Msg;

        public MessageHandleException()
        {
            
        }
        public MessageHandleException(BalloonTitleChanged msg)
        {
            Msg = msg;
        }
    }
}