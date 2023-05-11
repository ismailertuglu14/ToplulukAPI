using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Driver;
using Topluluk.Services.EventAPI.Data.Interface;
using Topluluk.Services.EventAPI.Model.Entity;

namespace Topluluk.Services.EventAPI.Data.Implementation
{
	public class EventCommentRepository : MongoGenericRepository<EventComment>, IEventCommentRepository
	{
		public IConnectionFactory _connectionFactory { get; set; }
		public EventCommentRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}
		private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

		private string GetCollectionName() => string.Format("{0}Collection", typeof(EventComment).Name);

		public async Task<Dictionary<string, int>> GetEventCommentCounts(List<string> eventIds)
		{
			var database = GetConnection();
			var collectionName = GetCollectionName();
			var filter = Builders<EventComment>.Filter.In(x => x.EventId, eventIds)
			             & Builders<EventComment>.Filter.Eq(x => x.IsDeleted, false);
			var comments = await database.GetCollection<EventComment>(collectionName).Find(filter).ToListAsync();

			var postCommentCounts = new Dictionary<string, int>();
			foreach (var eventId in eventIds)
			{
				var count = comments.Count(x => x.EventId == eventId);
				if (count > 0)
				{
					postCommentCounts.Add(eventId, count);
				}
			}

			return postCommentCounts;
		}
	}
}

