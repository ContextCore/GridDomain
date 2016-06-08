using System;
using System.Linq;
using GridDomain.CQRS.Quering;

namespace BusinessNews.ReadModel.Queries
{
    public class BusinessBalanceQuery : ISingleQuery<Guid, BusinessBalance>
    {
        private readonly IQueryable<BusinessBalance> _col;

        public BusinessBalanceQuery(IQueryable<BusinessBalance> col)
        {
            _col = col;
        }

        public BusinessBalance Execute(Guid businessId)
        {
            return _col.First(b => b.BusinessId == businessId);
        }
    }
}