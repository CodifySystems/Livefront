using CartonCaps.Application.Repositories;
using CartonCaps.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T>
    where T : BaseEntity
{
    // This is a placeholder for the actual implementation of the repository.
    // In a real application, this would interact with a database context.

    protected readonly DbContext Context;

    public BaseRepository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }
}