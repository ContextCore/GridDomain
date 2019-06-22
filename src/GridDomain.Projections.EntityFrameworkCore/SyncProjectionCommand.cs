using System;
using System.Threading.Tasks;
using GridDomain.EventHandlers;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Projections.EntityFrameworkCore
{
    
    public class SyncProjectionCommand
    {
        private readonly Func<IProjectionDbContext, IFindProjectionQuery> _queryFactory;
        private readonly string _projectorName;

        public SyncProjectionCommand(string projectorName, IProjectionDbContext context,
            Func<IProjectionDbContext, IFindProjectionQuery> queryFactory = null)
        {
            _context = context;
            _projectorName = projectorName;
            _queryFactory = queryFactory ?? (c => new FindProjectionQuery(c));
        }

        private readonly IProjectionDbContext _context;

        public async Task Execute(string projectionName, string eventName, long sequence, long version)
        {
            var projection = await _queryFactory(_context)
                .Execute(projectionName,
                    _projectorName,
                    eventName);

            if (projection == null)
                _context.Projections.Add(new Projection
                {
                    Event = eventName,
                    Name = projectionName,
                    Projector = _projectorName,
                    Offset = sequence,
                    Version = version
                });
            else
            {
                projection.Offset = sequence;
                projection.Version = version;
            }
        }
    }
}