using Microsoft.EntityFrameworkCore.Infrastructure;

namespace neuranx.Application.Interfaces;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}









































