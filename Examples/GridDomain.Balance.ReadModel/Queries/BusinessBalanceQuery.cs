using System;
using System.Linq;
using GridDomain.CQRS.Quering;

namespace BusinessNews.ReadModel.Queries
{
    public class BusinessBalanceQuery : ISingleQuery<Guid, BusinessAccount>
    {
        private readonly IQueryable<BusinessAccount> _col;

        public BusinessBalanceQuery(IQueryable<BusinessAccount> col)
        {
            _col = col;
        }

        public BusinessAccount Execute(Guid businessId)
        {
            return _col.First(b => b.BusinessId == businessId);
        }
    }
}