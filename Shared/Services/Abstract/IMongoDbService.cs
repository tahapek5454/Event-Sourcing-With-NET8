
using MongoDB.Driver;

namespace Shared.Services.Abstract
{
    public interface IMongoDbService
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}
