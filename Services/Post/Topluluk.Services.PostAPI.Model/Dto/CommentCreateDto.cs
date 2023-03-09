using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class CommentCreateDto : AbstractEntity
	{
		public string? UserId { get; set; }
		public string PostId { get; set; }
		public string Message { get; set; }
	}
}

