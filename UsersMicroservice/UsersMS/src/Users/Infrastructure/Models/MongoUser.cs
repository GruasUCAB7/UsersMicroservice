using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UsersMS.src.Users.Infrastructure.Models
{
    public class MongoUser
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }

        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public required string Email { get; set; }

        [BsonElement("phone"), BsonRepresentation(BsonType.String)]
        public required string Phone { get; set; }

        [BsonElement("userType"), BsonRepresentation(BsonType.String)]
        public required string UserType { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.Boolean)]
        public required bool Status { get; set; }

        [BsonElement("department"), BsonRepresentation(BsonType.String)]
        public required string Department { get; set; }
    }
}

