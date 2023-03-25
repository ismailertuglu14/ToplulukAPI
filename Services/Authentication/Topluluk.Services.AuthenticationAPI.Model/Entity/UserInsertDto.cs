using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.AuthenticationAPI.Model.Entity
{
	public class UserInsertDto
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthdayDate { get; set; }
        public GenderEnum? Gender { get; set; }
    }
}

