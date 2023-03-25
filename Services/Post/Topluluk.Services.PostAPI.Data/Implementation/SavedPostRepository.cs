using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

public class SavedPostRepository : MongoGenericRepository<SavedPost>, ISavedPostRepository
{
    public SavedPostRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
}