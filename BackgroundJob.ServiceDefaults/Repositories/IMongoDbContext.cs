using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BackgroundJob.Common.Repositories;

public interface IMongoDbContext
{
    public IMongoDatabase GetMongoDatabase(string databaseName);
    public IMongoCollection<T> GetMongoCollection<T>(string collectionName);
    public IMongoCollection<T> GetMongoCollection<T>(string collectionName, string databaseName = "");
}