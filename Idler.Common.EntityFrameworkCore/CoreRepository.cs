using Idler.Common.Core.Domain;

namespace Idler.Common.EntityFrameworkCore;

public class CoreRepository<TEntity, TKey> : BaseDBContextRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public CoreRepository(IDbContextFactory databaseFactory) : base(databaseFactory)
    {
    }
}