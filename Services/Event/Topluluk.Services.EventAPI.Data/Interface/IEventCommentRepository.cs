using System;
using DBHelper.Repository;
using Topluluk.Services.EventAPI.Model.Entity;

namespace Topluluk.Services.EventAPI.Data.Interface
{
	public interface IEventCommentRepository : IGenericRepository<EventComment>
	{
		Task<Dictionary<string, int>> GetEventCommentCounts(List<string> eventIds);
	}
}

