using System;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.EventAPI.Model.Dto
{
	public class CreateEventDto
	{
		// Tokendan gelecek
		public string? UserId { get; set; }


		// 
		public string? CommunityId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool? IsLimited { get; set; } = false;
		public int? AttendeesLimit { get; set; } = 0;
		public IFormFileCollection? Files { get; set; }
		public string? Location { get; set; }
		public DateTime? StartDate { get; set; } = DateTime.Now;
		public DateTime? EndDate { get; set; } = DateTime.Now.AddYears(1);

		public CreateEventDto()
		{
		}
	}
}

