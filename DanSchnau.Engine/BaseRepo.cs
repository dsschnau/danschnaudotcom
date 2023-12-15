using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace DanSchnau.Engine;

public abstract class BaseRepo
{
    protected IDbConnection Db { get; }
    protected IMemoryCache Cache { get; }

    public BaseRepo(IMemoryCache memoryCache)
    {
        this.Cache = memoryCache;
        this.Db = new DanSchnauConnection(Cache).GetProductionConnection();
    }
}
