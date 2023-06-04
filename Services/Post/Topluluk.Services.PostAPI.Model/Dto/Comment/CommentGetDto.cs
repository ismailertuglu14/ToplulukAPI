﻿using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class CommentGetDto
	{
		// Comment Id
		public string Id { get; set; }
		
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? ProfileImage { get; set; }
		public GenderEnum Gender { get; set; }

		public string Message { get; set; }

		public DateTime CreatedAt { get; set; }

		public int InteractionCount { get; set; }
		public bool IsLiked { get; set; } = false;

		// Replies lar farklı olacak. REply' a tekrardan REply atılamayacak.
		//public ICollection<CommentGetDto>? Replies { get; set; }

		public CommentGetDto()
		{
			//	Replies = new HashSet<CommentGetDto>();
		}
	}
}
