using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Data.SqlClient;

namespace DanSchnau.Engine;

public class DanSchnauConnection
{
    private IMemoryCache Cache { get; }

    public DanSchnauConnection(IMemoryCache memoryCache)
    {
        this.Cache = memoryCache;
    }

    public IDbConnection GetDevConnection()
    {
        var connectionString =
            "Data Source=.;Initial Catalog=DanSchnau-dev;Trusted_Connection=True;";
        return new SqlConnection(connectionString);
    }

    public IDbConnection GetProductionConnection()
    {

        var connectionString = string.Empty;
        if(!Cache.TryGetValue("connectionstring", out connectionString))
        {
            var vaultUri = new Uri("https://danschnaukeys.vault.azure.net/");
            var credential = new DefaultAzureCredential();
            var options = new SecretClientOptions() { };
            var keyVaultClient = new SecretClient(vaultUri, credential, options);

            var secret = keyVaultClient.GetSecret("danschnaudotcomdb");

            connectionString = secret.Value.Value;
            Cache.Set<string>("connectionstring", connectionString);

        }
        return new SqlConnection(connectionString);

    }
}
