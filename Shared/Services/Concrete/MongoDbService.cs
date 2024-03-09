using MongoDB.Driver;
using Shared.Services.Abstract;

namespace Shared.Services.Concrete
{
    public class MongoDbService(string connectionString, string dataBaseName) : IMongoDbService
    {
        private readonly string _connectionString = connectionString;
        private readonly string _databaseName = dataBaseName;

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            IMongoDatabase db = GetDatabase(_databaseName, _connectionString);
            return db.GetCollection<T>(collectionName);
        }

        private IMongoDatabase GetDatabase(string databaseName, string connectionString)
        {
            MongoClient mongoClient = new(connectionString);
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
