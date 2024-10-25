using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UsersMS.src.Users.Infrastructure.Models
{
    public class MongoDepartment
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }

        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public required string Description { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.Boolean)]
        public required bool Status { get; set; }

    }
}
