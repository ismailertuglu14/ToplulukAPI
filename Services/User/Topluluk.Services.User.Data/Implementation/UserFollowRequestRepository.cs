using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Model.Entity;

namespace Topluluk.Services.User.Data.Implementation;

public class UserFollowRequestRepository : MongoGenericRepository<FollowRequest>, IUserFollowRequestRepository
{
    private IConnectionFactory _connectionFactory;
    public UserFollowRequestRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
}