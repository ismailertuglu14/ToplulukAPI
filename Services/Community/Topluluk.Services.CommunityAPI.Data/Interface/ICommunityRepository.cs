using System;
using System.Linq.Expressions;
using DBHelper.Repository;
using DBHelper.Repository.Mongo;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Data.Interface
{
	public interface ICommunityRepository : IGenericRepository<Community>
	{
		Task<DatabaseResponse> AddItemToArrayProperty(string id, string arrayName, object item);
		Task<DatabaseResponse> RemoveItemFromArrayProperty<T>(string id, string arrayName, string addId);
		Task<Community> GetFirstCommunity(Expression<Func<Community, bool>> predicate);
    }
}

