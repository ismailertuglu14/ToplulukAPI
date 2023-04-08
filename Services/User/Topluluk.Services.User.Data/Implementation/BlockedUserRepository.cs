using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Model.Entity;

namespace Topluluk.Services.User.Data.Implementation;

public class BlockedUserRepository : MongoGenericRepository<BlockedUser>, IBlockedUserRepository
{
    public BlockedUserRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
}