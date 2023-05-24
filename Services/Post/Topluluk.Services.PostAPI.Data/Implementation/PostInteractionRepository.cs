using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Data.Implementation;

public class PostInteractionRepository : MongoGenericRepository<PostInteraction>, IPostInteractionRepository
{
    private readonly IMongoCollection<PostInteraction> _collection;
    private readonly IConnectionFactory _connectionFactory;
    public PostInteractionRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _collection = GetCollection();

    }
    private IMongoDatabase GetConnection()
    {
        return (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;
    }

    private IMongoCollection<PostInteraction> GetCollection()
    {
        var database = GetConnection();
        var collectionName = typeof(PostInteraction).Name + "Collection";
        return database.GetCollection<PostInteraction>(collectionName);
    }

    public async Task<Dictionary<string, PostInteraction>> IsUserInteractedPosts(string userId, List<string> postIds)
    {
        var postInteractionDict = new Dictionary<string, PostInteraction>();

        var interactedPosts = _collection.Find(p => postIds.Contains(p.PostId) && p.UserId == userId).ToList();
        
        foreach (var postInteraction in interactedPosts)
        {
            postInteractionDict[postInteraction.PostId] = postInteraction;
        }

        return postInteractionDict;
    }

    public async Task<Dictionary<string, int>> PostsInteractionCounts(List<string> postIds)
    {
        var postInteractionCountDict = new Dictionary<string, int>();
        var pipeline = new BsonDocument[] {
            new BsonDocument("$match", new BsonDocument {
                {"PostId", new BsonDocument {{"$in", new BsonArray(postIds)}}},
                {"IsDeleted", false}
            }),
            new BsonDocument("$group", new BsonDocument {
                {"_id", "$PostId"},
                {"Count", new BsonDocument {{"$sum", 1}}}
            })
        };
        var cursor = await _collection.AggregateAsync<BsonDocument>(pipeline);
        var results = await cursor.ToListAsync();
        foreach (var result in results)
        {
            postInteractionCountDict.Add(result["_id"].AsString, result["Count"].AsInt32);
        }
        return postInteractionCountDict;
    }

    public async Task<Dictionary<string, PostInteractionPreviewDto[]>> GetPostInteractionPreviews(List<string> postIds)
    {
        var postInteractionPreviews = new Dictionary<string, PostInteractionPreviewDto[]>();

        var filter = Builders<PostInteraction>.Filter.And(
            Builders<PostInteraction>.Filter.In("PostId", postIds),
            Builders<PostInteraction>.Filter.Eq("IsDeleted", false)
        );

        var projection = Builders<PostInteraction>.Projection
            .Include(i => i.PostId)
            .Include(i => i.InteractionType);

        var cursor = await _collection.FindAsync(filter, new FindOptions<PostInteraction, BsonDocument> { Projection = projection });
        var result = await cursor.ToListAsync();

        foreach (var postId in postIds)
        {
            var interactions = result
                .Where(x => x["PostId"] == postId)
                .GroupBy(x => x["InteractionType"].AsInt32)
                .OrderByDescending(x => x.Count())
                .Take(3)
                .Select(x => new PostInteractionPreviewDto()
                {
                    Interaction = (InteractionEnum)x.Key,
                    InteractionCount = x.Count()
                })
                .ToArray();

            postInteractionPreviews.Add(postId, interactions);
        }

        return postInteractionPreviews;
    }


}