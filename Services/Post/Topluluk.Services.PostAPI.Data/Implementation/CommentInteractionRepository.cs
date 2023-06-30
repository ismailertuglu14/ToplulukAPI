using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Implementation;

public class CommentInteractionRepository : MongoGenericRepository<CommentInteraction>, ICommentInteractionRepository
{
    public CommentInteractionRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
}