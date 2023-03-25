using DBHelper.Repository;
using Topluluk.Services.ChatAPI.Model.Entity;

namespace Topluluk.Services.ChatAPI.Data.Interface;

public interface IChatRepository : IGenericRepository<Message>
{
    
}