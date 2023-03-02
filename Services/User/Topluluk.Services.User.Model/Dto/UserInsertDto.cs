using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Topluluk.Services.User.Model.Dto
{
	public class UserInsertDto
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public DateTime BirthdayDate { get; set; }
	}
}

