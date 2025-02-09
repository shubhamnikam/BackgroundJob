using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BackgroundJob.Common.Repositories;

public class MongoDbContext : IMongoDbContext
{
    private readonly MongoClient _mongoClient;
    private readonly IMongoDatabase _mongoDatabase;
    public MongoDbContext(IConfiguration configuration)
    {
        _mongoClient = new MongoClient(configuration.GetValue<string>("MongoDB:DefaultConnection"));
        _mongoDatabase = GetMongoDatabase(configuration.GetValue<string>("MongoDB:DefaultDatabase"));
    }
    public IMongoDatabase GetMongoDatabase(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name cannot be null or empty", nameof(databaseName));
        }
        return _mongoClient.GetDatabase(databaseName);
    }
    public IMongoCollection<T> GetMongoCollection<T>(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new ArgumentException("Database collection name cannot be null or empty", nameof(collectionName));
        }

        return _mongoDatabase.GetCollection<T>(collectionName);
    }

    public IMongoCollection<T> GetMongoCollection<T>(string collectionName, string databaseName)
    {
        if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database/collection name cannot be null or empty", nameof(collectionName));
        }

        var database = GetMongoDatabase(databaseName);
        return database.GetCollection<T>(collectionName);
    }
}