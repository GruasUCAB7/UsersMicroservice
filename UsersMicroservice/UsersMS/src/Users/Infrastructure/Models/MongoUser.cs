using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UsersMS.src.Users.Infrastructure.Models
{
    public class MongoUser
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("email")]
        public required string Email { get; set; }

        [BsonElement("phone")]
        public required string Phone { get; set; }

        [BsonElement("department")]
        public required string Department { get; set; }

        [BsonElement("passwordHash")]
        public required string PasswordHash { get; set; }

        [BsonElement("userType")]
        public required string UserType { get; set; }

        [BsonElement("isTemporaryPassword")]
        public bool IsTemporaryPassword { get; set; }

        [BsonElement("passwordExpirationDate")]
        public DateTime? PasswordExpirationDate { get; set; }

        [BsonElement("isActive")]
        public required bool IsActive { get; set; }

        [BsonElement("createdDate")]
        public required DateTime CreatedDate { get; set; }

        [BsonElement("updatedDate")]
        public required DateTime UpdatedDate { get; set; }
    }
}
