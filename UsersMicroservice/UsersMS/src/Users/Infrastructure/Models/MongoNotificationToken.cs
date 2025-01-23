using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UsersMS.src.Users.Infrastructure.Models
{
    public class MongoNotificationToken
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("userId")]
        public required string UserId { get; set; }

        [BsonElement("token")]
        public required string Token { get; set; }
    }
}
