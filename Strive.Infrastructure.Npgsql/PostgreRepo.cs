//using Microsoft.Extensions.Options;

namespace Strive.Infrastructure.Data.Mongo
{
    public abstract class PostgreRepo<T>
    {
        protected readonly ICollection<T> Collection;
       // protected readonly MongoClient MongoClient;

        protected PostgreRepo()
        {
            //MongoClient = new MongoClient(options.Value.ConnectionString);
            //var database = MongoClient.GetDatabase(options.Value.DatabaseName);
            //Collection = database.GetCollection<T>(options.Value.CollectionNames[typeof(T).Name]);
        }
    }
}