using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.EventAPI.Data.Interface;
using Topluluk.Services.EventAPI.Model.Entity;

namespace Topluluk.Services.EventAPI.Data.Implementation;

public class EventAttendeesRepository : MongoGenericRepository<EventAttendee>, IEventAttendeesRepository
{
    public EventAttendeesRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
}