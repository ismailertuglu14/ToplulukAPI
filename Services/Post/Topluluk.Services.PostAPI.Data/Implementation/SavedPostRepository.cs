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

    public async Task<Dictionary<string, bool>> IsUserSavedPosts(string userId, List<string> postIds)
    {
        var database = GetConnection();
        var collectionName = GetCollectionName();

        var filter = Builders<SavedPost>.Filter.And(
            Builders<SavedPost>.Filter.In(p => p.PostId, postIds),
            Builders<SavedPost>.Filter.Eq(p => p.UserId, userId),
            Builders<SavedPost>.Filter.Eq(p => p.IsDeleted, false)
        );
        var projection = Builders<SavedPost>.Projection
            .Include("PostId")
            .Exclude("_id");

        var result = database.GetCollection<SavedPost>(collectionName).Find(filter)
            .Project(projection)
            .ToList();
        
        var postsInformation = new Dictionary<string, bool>();

        foreach (var document in result)
        {
            var postId = document["PostId"].AsString;
            postsInformation[postId] = true; // Saved olduğunu varsayıyoruz
        }

        return postsInformation;
    }
}