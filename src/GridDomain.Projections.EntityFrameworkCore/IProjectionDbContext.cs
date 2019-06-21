using Microsoft.EntityFrameworkCore;

namespace GridDomain.Projections.EntityFrameworkCore
{
    public interface IProjectionDbContext
    {
        DbSet<Projection> Projections { get; set; }
    }
}