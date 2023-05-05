﻿using System;
using DBHelper.Repository;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Interface
{
	public interface IPostCommentRepository : IGenericRepository<PostComment>
	{
		Task<bool> DeletePostsComments(string userId);
		Task<Dictionary<string, int>> GetPostCommentCounts(List<string> postIds);

	}
}

