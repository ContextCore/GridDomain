using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Commands;

namespace GridDomain.Tests.Unit.BalloonDomain
{
    public class BalloonCommandHandler : AggregateCommandsHandler<Balloon>

    {
        public BalloonCommandHandler()
        {
            Map<WriteTitleCommand>((c, a) => a.WriteNewTitle(c.Parameter));

            Map<IncreaseTitleCommand>((c, a) => a.IncreaseTitle(c.Value));

            Map<InflateNewBallonCommand>(c => new Balloon(c.AggregateId, c.Title.ToString()));

            Map<InflateCopyCommand>((c, a) => a.InflateNewBaloon(c.Parameter.ToString()));

            Map<PlanTitleWriteCommand>((c, a) => a.PlanTitleWrite(c.Parameter));

            Map<PlanTitleChangeCommand>((c, a) => a.PlanTitleWrite(c.Parameter, c.SleepTime));

            Map<BlowBalloonCommand>((c, a) => a.Blow());

            Map<PlanBallonBlowCommand>((c, a) => a.BlowAfter(c.SleepTime));

            Map<PlanTitleWriteAndBlowCommand>((c, a) => a.PlanWriteTitleToBlow(c.Parameter, c.SleepTime));
        }
    }
}