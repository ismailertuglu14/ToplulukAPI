using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.EventAPI.Data.Interface;
using Topluluk.Services.EventAPI.Model.Entity;

namespace Topluluk.Services.EventAPI.Data.Implementation
{
	public class EventCommentRepository : MongoGenericRepository<EventComment>, IEventCommentRepository
	{
		public EventCommentRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
		}
	}
}

