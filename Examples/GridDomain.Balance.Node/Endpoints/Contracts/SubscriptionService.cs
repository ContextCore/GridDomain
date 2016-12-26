using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessNews.Domain.BusinessAggregate;
using GridDomain.CQRS.Quering;
using GridDomain.Node;

namespace BusinessNews.Node.Endpoints.Contracts
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IGridDomainNode _node;
        private readonly ISingleQuery<Guid, SubscriptionViewModel> _businessCurrentSubscriptionQuery;
        private readonly IQuery<OfferViewModel> _availableSubscriptionsQuery;

        public SubscriptionService(
            IGridDomainNode node,
            ISingleQuery<Guid,SubscriptionViewModel> businessCurrentSubscriptionQuery,
            IQuery<OfferViewModel> availableSubscriptionsQuery )
        {
            _node = node;
            _businessCurrentSubscriptionQuery = businessCurrentSubscriptionQuery;
            _availableSubscriptionsQuery = availableSubscriptionsQuery;
        }

        public Task SubscribeBusiness(Guid businessId, Guid offerId)
        {
            _node.Execute(new OrderSubscriptionCommand(businessId,offerId,Guid.NewGuid()));
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<OfferViewModel>> ListSubscriptions()
        {
            return Task.FromResult(_availableSubscriptionsQuery.Execute());
        }

        public Task<SubscriptionViewModel> GetBusinessSubscription(Guid businessId)
        {
            return Task.FromResult(_businessCurrentSubscriptionQuery.Execute(businessId));
        }
    }
}