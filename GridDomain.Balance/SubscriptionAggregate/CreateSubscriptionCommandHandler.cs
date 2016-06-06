using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class CreateSubscriptionCommandHandler : AggregateCommandsHandler<Subscription>
    {
        public CreateSubscriptionCommandHandler()
        {
         //   Map<CreateSubscriptionCommand>(cmd => cmd.SubscriptionId,
         //       cmd => new Subscription(cmd.SubscriptionId, cmd.Offer));
        }
    }
}