﻿using System;
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

		public string? Location { get; set; }

		public int ParticipiantLimit { get; set; }
		public bool IsLimited { get; set; } = false;


        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; } = DateTime.Now.AddYears(1);

        public ICollection<InteractionType> Interactions { get; set; }

		public Event()
		{
			Interactions = new HashSet<InteractionType>();
            Images = new List<string>();
		}
	}
}

