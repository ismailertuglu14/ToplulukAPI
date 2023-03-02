using System;
using DBHelper.Repository;
using Topluluk.Shared.Dtos;
using _User = Topluluk.Services.User.Model.Entity.User;
namespace Topluluk.Services.User.Data.Interface
{
	public interface IUserRepository : IGenericRepository<_User>
	{
	}
}

