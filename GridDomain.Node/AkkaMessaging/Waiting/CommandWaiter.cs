using Akka;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandWaiter : AnyMessageWaiter
    {
        private readonly ICommand _command;

        public CommandWaiter(IActorRef notifyActor, ICommand command, params ExpectedMessage[] expectedMessage) : base(notifyActor, expectedMessage)
        {
            _command = command;
        }

        protected override object BuildAnswerMessage(object message)
        {
            object answerMessage = null;
            message.Match()
                .With<ICommandFault>(f => answerMessage = f)
                .Default(m =>
                {
                    answerMessage = new CommandExecutionFinished(_command, m);
                });

            return answerMessage;
        }
    }
}