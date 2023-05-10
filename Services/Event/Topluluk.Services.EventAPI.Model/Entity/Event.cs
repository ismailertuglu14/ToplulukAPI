using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Model.Entity
{
	public class Event : AbstractEntity
	{
		public string UserId { get; set; }
		
		public string Title { get; set; }
		public string Description { get; set; }

		public List<string>? Images { get; set; }

		public string? CommunityId { get; set; }

		public bool IsLocationOnline { get; set; }
		public string? LocationPlace { get; set; }
		public string? LocationURL { get; set; }

		public int ParticipiantLimit { get; set; }
        public bool IsLimited { get; set; }

        public bool IsExpired { get; set; } = false;

		public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; } = DateTime.Now.AddHours(1);

		public ICollection<string> Attendees { get; set; }
		public List<string> Sponsors { get; set; }

		// Everyone can see or only those with link
		public bool IsVisible { get; set; }
		
		public bool IsPaid { get; set; }
		public double Price { get; set; }
		public Event()
		{
            Images = new List<string>();
			Attendees = new HashSet<string>();
			Sponsors = new List<string>();
		}
	}
	
}

