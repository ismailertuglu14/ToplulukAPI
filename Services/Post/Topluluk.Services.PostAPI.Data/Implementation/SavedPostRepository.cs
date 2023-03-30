using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

public class SavedPostRepository : MongoGenericRepository<SavedPost>, ISavedPostRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SavedPostRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

    private string GetCollectionName() => string.Format("{0}Collection", typeof(SavedPost).Name);

    public async Task<bool> DeleteSavedPostsByUserId(string userId)
    {
        try
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var filter = Builders<SavedPost>.Filter.And(
                Builders<SavedPost>.Filter.Eq(p => p.UserId, userId),
                Builders<SavedPost>.Filter.Eq(p => p.IsDeleted, false));

            var update = Builders<SavedPost>.Update.Set(p => p.IsDeleted, true);

            database.GetCollection<SavedPost>(collectionName).UpdateMany(filter, update);
            return await Task.FromResult(true);
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }
}