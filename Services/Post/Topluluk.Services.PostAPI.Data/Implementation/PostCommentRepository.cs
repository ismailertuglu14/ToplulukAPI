using System;
using System.Linq.Expressions;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Implementation
{
	public class PostCommentRepository: MongoGenericRepository<PostComment>, IPostCommentRepository
	{
        private readonly IConnectionFactory _connectionFactory;

        public PostCommentRepository(IConnectionFactory connection) : base(connection)
		{
            _connectionFactory = connection;
        }
        private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

        private string GetCollectionName() => $"{nameof(PostComment)}Collection";

        public async Task<List<PostComment>> GetPostCommentsDescendingDate(int skip, int take, Expression<Func<PostComment, bool>> predicate)
        {
            try
            {
                var database = GetConnection();
                var collectionName = GetCollectionName();
                var sort = Builders<PostComment>.Sort.Descending(p => p.CreatedAt);
                var cursor = await database.GetCollection<PostComment>(collectionName)
                    .Find(predicate)
                    .Sort(sort)
                    .Skip(skip * take)
                    .Limit(take)
                    .ToCursorAsync();
                
                return await cursor.ToListAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> DeletePostsComments(string userId)
        {
            try
            {
                var database = GetConnection();
                var collectionName = GetCollectionName();

                var filter = Builders<PostComment>.Filter.And(
                    Builders<PostComment>.Filter.Eq(p => p.UserId, userId),
                    Builders<PostComment>.Filter.Eq(p => p.IsDeleted, false));

                var update = Builders<PostComment>.Update.Set(p => p.IsDeleted, true);

                database.GetCollection<PostComment>(collectionName).UpdateMany(filter, update);
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<Dictionary<string, int>> GetPostCommentCounts(List<string> postIds)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();
            var filter = Builders<PostComment>.Filter.In(x => x.PostId, postIds)
                         & Builders<PostComment>.Filter.Eq(x => x.IsDeleted, false)
                         & Builders<PostComment>.Filter.Eq( x => x.ParentCommentId, null);
                
            var comments = await database.GetCollection<PostComment>(collectionName).Find(filter).ToListAsync();

            var postCommentCounts = new Dictionary<string, int>();
            foreach (var postId in postIds)
            {
                var count = comments.Count(x => x.PostId == postId);
                if (count > 0)
                {
                    postCommentCounts.Add(postId, count);
                }
            }

            return postCommentCounts;
        }

        public async Task<Dictionary<string, int>> GetCommentsReplyCounts(List<string> commentIds)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();
            var filter = Builders<PostComment>.Filter.In(x => x.ParentCommentId, commentIds)
                         & Builders<PostComment>.Filter.Eq(x => x.IsDeleted, false);
            var comments = await database.GetCollection<PostComment>(collectionName).Find(filter).ToListAsync();

            var commentReplyCounts = new Dictionary<string, int>();
            
            foreach (var commentId in commentIds)
            {
                var count = comments.Count(x => x.ParentCommentId == commentId);
                if (count > 0)
                {
                    commentReplyCounts.Add(commentId, count);
                }
            }

            return commentReplyCounts;
        }
    }
}

