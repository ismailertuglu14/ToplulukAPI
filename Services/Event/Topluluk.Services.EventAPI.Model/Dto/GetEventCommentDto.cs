using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.EventAPI.Model.Dto
{
	public class GetEventCommentDto
	{
		public string Id { get; set; }

		public string UserId { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? ProfileImage { get; set; }

		public string Message { get; set; }

		public int InteractionCount { get; set; }

		public bool IsInteracted { get; set; } = false;
        public GenderEnum Gender { get; set; }

        public GetEventCommentDto()
		{
		}
	}
}

