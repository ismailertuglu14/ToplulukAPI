using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Entity;

namespace Topluluk.Services.CommunityAPI.Data.Implementation
{
	public class CommunityParticipiantRepository : MongoGenericRepository<CommunityParticipiant>, ICommunityParticipiantRepository
	{
		public CommunityParticipiantRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}
	}
}

