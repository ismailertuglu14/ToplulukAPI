using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.ChatAPI.Data.Interface;
using Topluluk.Services.ChatAPI.Model.Entity;

namespace Topluluk.Services.ChatAPI.Data.Implementation;

public class ChatRepository : MongoGenericRepository<Message> , IChatRepository
{
    public ChatRepository(IConnectionFactory connection) : base(connection)
    {
        
    }
}