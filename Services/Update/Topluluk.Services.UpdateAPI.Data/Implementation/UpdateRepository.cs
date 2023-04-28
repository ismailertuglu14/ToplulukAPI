using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Topluluk.Services.UpdateAPI.Data.Interface;
using Topluluk.Services.UpdateAPI.Model.Entity;

namespace Topluluk.Services.UpdateAPI.Data.Implementation;

public class UpdateRepository : MongoGenericRepository<AppVersion>, IUpdateRepository
{
    IConnectionFactory _connection;
    public UpdateRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _connection = connectionFactory;
    }
    private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connection.GetConnection;

    private string GetCollectionName() => string.Format("{0}Collection", typeof(AppVersion).Name);


    public async Task<AppVersion> GetLatestVersion()
    {
        var database = GetConnection();
        var collectionName = GetCollectionName();

        var collection = database.GetCollection<AppVersion>(collectionName);
        var filter = Builders<AppVersion>.Filter.Empty;
        var sort = Builders<AppVersion>.Sort.Descending(x => x.CreatedAt);
        var latestVersion = await collection.Find(filter).Sort(sort).FirstOrDefaultAsync();
        return latestVersion;
    }

    public async Task<bool> CheckIfUpdateIsRequired(double currentVersion)
    {
        var database = GetConnection();
        var collectionName = GetCollectionName();

        var collection = database.GetCollection<AppVersion>(collectionName);
        var filter = Builders<AppVersion>.Filter.And(
            Builders<AppVersion>.Filter.Gt("Version", currentVersion),
            Builders<AppVersion>.Filter.Or(
                Builders<AppVersion>.Filter.Eq("IsRequired", true),
                Builders<AppVersion>.Filter.Not(Builders<AppVersion>.Filter.Exists("IsRequired"))
            )
        );

        var count = await collection.Find(filter).Limit(1).CountDocumentsAsync();
        return count > 0;
    }
}