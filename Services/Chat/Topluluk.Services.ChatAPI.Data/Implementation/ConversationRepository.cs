using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.ChatAPI.Model.Entity;

namespace Topluluk.Services.ChatAPI.Data.Implementation;

public class ConversationRepository : MongoGenericRepository<Conversation>
{
    public ConversationRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
}