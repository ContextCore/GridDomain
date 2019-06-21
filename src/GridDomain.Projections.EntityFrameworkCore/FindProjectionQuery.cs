using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Projections.EntityFrameworkCore
{
    public class FindProjectionQuery : IFindProjectionQuery
    {
        private readonly IProjectionDbContext _context;

        public FindProjectionQuery(IProjectionDbContext context)
        {
            _context = context;
        }

        public Task<Projection> Execute(string name, string projector, string eventName)
        {
            return _context.Projections.SingleOrDefaultAsync(p => p.Name == name &&
                                                             p.Event == eventName &&
                                                             p.Projector == projector);
        }
    }
}