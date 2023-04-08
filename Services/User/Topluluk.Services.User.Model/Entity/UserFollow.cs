using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Model.Entity;

public class UserFollow : AbstractEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SourceId { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string TargetId { get; set; }
}