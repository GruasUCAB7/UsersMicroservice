using MongoDB.Bson;
using MongoDB.Driver;

namespace UsersMS.Core.Infrastructure.Data
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService()
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
            var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName), "MongoDB database name cannot be null or empty.");
            }

            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<BsonDocument> GetUserCollection()
        {
            return _database.GetCollection<BsonDocument>("user");
        }
    }
}
