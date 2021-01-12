using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.MongoInit
{
    public interface IMongoDBInit
    {
        Task<IMongoDatabase> InitializeDb();
    }
}