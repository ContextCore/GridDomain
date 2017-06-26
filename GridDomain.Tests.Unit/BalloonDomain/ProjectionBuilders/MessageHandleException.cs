using System;
using GridDomain.Tests.XUnit.BalloonDomain.Events;

namespace GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders
{
    public class MessageHandleException : Exception
    {
        public readonly BalloonTitleChanged Msg;

        public MessageHandleException(BalloonTitleChanged msg)
        {
            Msg = msg;
        }
    }
}