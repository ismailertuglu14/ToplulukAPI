using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Driver;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Entity;

namespace Topluluk.Services.CommunityAPI.Data.Implementation
{
	public class CommunityParticipiantRepository : MongoGenericRepository<CommunityParticipiant>, ICommunityParticipiantRepository
	{
		private readonly IConnectionFactory _connectionFactory;
		public CommunityParticipiantRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}
		private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

		private string GetCollectionName() => string.Format("{0}Collection", typeof(CommunityParticipiant).Name);

		// Community nin participiantsları
		public async Task<Dictionary<string, int>> GetCommunityParticipiants(List<string> communityIds)
		{
			var database = GetConnection();
			var collectionName = GetCollectionName();
			var filter = Builders<CommunityParticipiant>.Filter.In(x => x.CommunityId,communityIds)
			             & Builders<CommunityParticipiant>.Filter.Eq(x => x.IsDeleted, false);
			var participiants  = await database.GetCollection<CommunityParticipiant>(collectionName).Find(filter).ToListAsync();

			var communityParicipiantCounts = new Dictionary<string, int>();
			foreach (var communityId in communityIds)
			{
				var count = participiants.Count(x => x.CommunityId == communityId);
				if (count > 0)
				{
					communityParicipiantCounts.Add(communityId, count);
				}
			}

			return communityParicipiantCounts;
		}
	}
}

