﻿using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Model.Entity
{
	public class EventComment : AbstractEntity
	{
		public string EventId { get; set; }
		public string UserId { get; set; }
		public string Message { get; set; }
        public ICollection<InteractionType> Interactions { get; set; }

        public EventComment()
		{
			Interactions = new HashSet<InteractionType>();
        }
    }
}

