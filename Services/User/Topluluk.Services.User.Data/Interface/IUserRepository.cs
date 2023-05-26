using System;
using DBHelper.Repository;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Shared.Dtos;
using _User = Topluluk.Services.User.Model.Entity.User;
namespace Topluluk.Services.User.Data.Interface
{
	public interface IUserRepository : IGenericRepository<_User>
	{
		/// <summary>
		/// If userName is exist returns true,
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
        Task<bool> CheckIsUsernameUnique(string userName);
        Task<bool> CheckIsEmailUnique(string email);
    }
}

