using System;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.EventAPI.Model.Dto
{
	public class CreateEventDto
	{
		// 
		public string? CommunityId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool? IsLimited { get; set; } = false;
		public int? ParticipiantLimit { get; set; } = 0;
		public IFormFileCollection? Files { get; set; }
		
		public bool IsLocationOnline { get; set; }
		public string? Location { get; set; }
		public DateTime? StartDate { get; set; } = DateTime.Now;
		public DateTime? EndDate { get; set; } = DateTime.Now.AddYears(1);
		
		public bool IsPaid { get; set; }
		public double Price { get; set; }

		public bool IsVisible { get; set; }
		
		public CreateEventDto()
		{
		}
	}
}
