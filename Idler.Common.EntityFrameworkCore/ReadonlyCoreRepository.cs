using Idler.Common.Core.Domain;

namespace Idler.Common.EntityFrameworkCore;

public class ReadonlyCoreRepository<TEntity, TKey> : ReadonlyBaseDbContextRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public ReadonlyCoreRepository(IDbContextFactory databaseFactory) : base(databaseFactory)
    {
    }
}