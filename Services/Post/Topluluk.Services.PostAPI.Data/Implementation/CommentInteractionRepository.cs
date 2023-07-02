using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Implementation;

public class CommentInteractionRepository : MongoGenericRepository<CommentInteraction>, ICommentInteractionRepository
{
    private readonly IConnectionFactory _connectionFactory;

  
    public CommentInteractionRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {

        _connectionFactory = connectionFactory;
    }
    private string GetCollectionName() => $"{nameof(CommentInteraction)}Collection";
    private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;
    
    
    public async Task<Dictionary<string, CommentLikes>> GetCommentsInteractionCounts(List<string> commentIds)
    {
        var database = GetConnection();
        var collectionName = GetCollectionName();

        var filter = Builders<CommentInteraction>.Filter.Eq(c => c.IsDeleted, false)
                     & Builders<CommentInteraction>.Filter.In(c => c.CommentId, commentIds);

        Dictionary<string, CommentLikes> commentsInteractionCounts = new();

        var projection = Builders<CommentInteraction>.Projection
            .Include(c => c.CommentId)
            .Include(c => c.Type);
        
        var interactions = await database.GetCollection<CommentInteraction>(collectionName)
            .Find(filter)
            .Project<CommentInteraction>(projection)
            .ToListAsync();
        
        foreach (var interaction in interactions)
        {
            if (!commentsInteractionCounts.ContainsKey(interaction.CommentId))
            {
                var likeCount = interactions.Count(x => x.CommentId == interaction.CommentId && x.Type == CommentInteractionType.LIKE);
                var dislikeCount = interactions.Count(x => x.CommentId == interaction.CommentId && x.Type == CommentInteractionType.DISLIKE);
                commentsInteractionCounts.Add(interaction.CommentId, new CommentLikes
                {
                    LikeCount = likeCount,
                    DislikeCount = dislikeCount
                } );
            }
        }

        return commentsInteractionCounts;
    }

    public async Task<Dictionary<string, CommentInteracted>> GetCommentsIsInteracted(string userId, List<string> commentIds)
    {
        var database = GetConnection();
        var collectionName = GetCollectionName();

        var filter = Builders<CommentInteraction>.Filter.And(
            Builders<CommentInteraction>.Filter.Eq(c => c.IsDeleted, false),
            Builders<CommentInteraction>.Filter.In(c => c.CommentId, commentIds),
            Builders<CommentInteraction>.Filter.Eq(c => c.UserId, userId)
        );

        var projection = Builders<CommentInteraction>.Projection
            .Include(c => c.CommentId)
            .Include(c =>c.UserId)
            .Include(c => c.Type);

        var interactionsCursor = await database.GetCollection<CommentInteraction>(collectionName)
            .Find(filter)
            .Project<CommentInteraction>(projection)
            .ToCursorAsync();

        var commentsInteracted = new Dictionary<string, CommentInteracted>();

        while (await interactionsCursor.MoveNextAsync())
        {
            var interactions = interactionsCursor.Current;
            foreach (var interaction in interactions)
            {
                var commentId = interaction.CommentId;

                if (!commentsInteracted.ContainsKey(commentId))
                {
                    var isLiked = interactions.Any(x =>
                        x.CommentId == commentId && x.UserId == userId && x.Type == CommentInteractionType.LIKE);

                    var isDisliked = interactions.Any(x =>
                        x.CommentId == commentId && x.UserId == userId && x.Type == CommentInteractionType.DISLIKE);

                    commentsInteracted.Add(commentId, new CommentInteracted
                    {
                        Like = isLiked,
                        Dislike = isDisliked
                    });
                }
            }
        }

        return commentsInteracted;
    }
    
}