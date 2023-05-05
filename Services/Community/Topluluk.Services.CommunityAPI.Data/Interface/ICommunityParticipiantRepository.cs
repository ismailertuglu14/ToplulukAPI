using System;
using DBHelper.Repository;
using Topluluk.Services.CommunityAPI.Model.Entity;

namespace Topluluk.Services.CommunityAPI.Data.Interface
{
	
	public interface ICommunityParticipiantRepository : IGenericRepository<CommunityParticipiant>
	{
		Task<Dictionary<string, int>> GetCommunityParticipiants(List<string> communityIds);
	}
}

