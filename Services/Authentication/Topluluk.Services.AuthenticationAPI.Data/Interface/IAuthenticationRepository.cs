using System;
using DBHelper.Repository;
using Topluluk.Services.AuthenticationAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Data.Interface
{
	public interface IAuthenticationRepository : IGenericRepository<UserCredential>
	{
		DatabaseResponse UpdateRefreshToken(UserCredential entity);

    }
}

