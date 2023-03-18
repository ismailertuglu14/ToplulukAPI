using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.EventAPI.Data.Interface;
using Topluluk.Services.EventAPI.Model.Entity;

namespace Topluluk.Services.EventAPI.Data.Implementation
{
	public class EventRepository : MongoGenericRepository<Event> , IEventRepository
	{
		public EventRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}
	}
}

