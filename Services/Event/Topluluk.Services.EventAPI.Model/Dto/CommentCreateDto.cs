using System;
namespace Topluluk.Services.EventAPI.Model.Dto
{
	public class CommentCreateDto
	{
		public string Message { get; set; }
		public string EventId { get; set; }

		public CommentCreateDto()
		{
		}
	}
}

