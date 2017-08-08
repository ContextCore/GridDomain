using System;

namespace GridDomain.Node.Actors.Hadlers {
    public class MessageProcessFailedException : Exception
    {
        public MessageProcessFailedException(Exception exception):base("Message process caused an error",exception)
        {
        }
    }
}