using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Implementation;

public class PostInteractionRepository : MongoGenericRepository<PostInteraction>, IPostInteractionRepository
{
    public PostInteractionRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
}