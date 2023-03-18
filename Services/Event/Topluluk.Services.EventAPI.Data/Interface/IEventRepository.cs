using System;
using DBHelper;
using Topluluk.Services.EventAPI.Model.Entity;
using DBHelper.Repository;

namespace Topluluk.Services.EventAPI.Data.Interface
{
	public interface IEventRepository : IGenericRepository<Event>
	{
		
	}
}

