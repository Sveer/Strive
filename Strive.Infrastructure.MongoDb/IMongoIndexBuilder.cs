using System.Threading.Tasks;

namespace Strive.Infrastructure.Data.Mongo
{
    public interface IMongoIndexBuilder
    {
        Task CreateIndexes();
    }
}
