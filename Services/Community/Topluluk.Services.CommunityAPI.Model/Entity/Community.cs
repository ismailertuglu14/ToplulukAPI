using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Model.Entity
{
	public class Community : AbstractEntity
	{
		public string? AdminId { get; set; }

		public ICollection<Moderator> ModeratorIds { get; set; }
		public string Title { get; set; }
		public string Slug { get; set; }
		public string Description { get; set; }
		public string? CoverImage { get; set; }
		public string? BannerImage { get; set; }
		// Herkes topluluğu görebilecek mi ?
		public bool IsVisible { get; set; } = true;
		// Kullanıcılar topluluğa katılabilmek için izin istemek zorunda mı ?
		public bool IsPublic { get; set; } = true;
		public bool IsRestricted { get; set; } = false;
		public int? ParticipiantLimit { get; set; }
		public string? Location { get; set; }


		public ICollection<Question> Questions { get; set; }

		public ICollection<string> Cities { get; set; }

		public ICollection<string> BlockedUsers { get; set; }

		//public ICollection<string> Posts { get; set; }

		//public bool HasPrice { get; set; } = false;
		//public int Price { get; set; } = 0;

		public Community()
		{
			ModeratorIds = new HashSet<Moderator>();
			Questions = new HashSet<Question>();
			Cities = new HashSet<string>();
			BlockedUsers = new HashSet<string>();
		}
	}
	public class Moderator
	{
		public string UserId { get; set; }
		public string AssignedById { get; set; }
	}
	public class Question
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public bool IsRequired { get; set; } = true;
	}
}

