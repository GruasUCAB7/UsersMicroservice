using MongoDB.Bson;
using MongoDB.Driver;
using UsersMS.Core.Infrastructure.Data;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Infrastructure.Models;

namespace UsersMS.src.Users.Infrastructure.Repositories
{
    public class MongoUserRepository(MongoDbService mongoDbService) : IUserRepository
    {
        private readonly IMongoCollection<BsonDocument> _userCollection = mongoDbService.GetUserCollection();

        public async Task<bool> ExistByEmail(string email)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var result = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }

        public async Task<Result<User>> Save(User user)
        {
            var mongoUser = new MongoUser
            {
                Id = user.GetId().GetValue(),
                Name = user.GetName().GetValue(),
                Email = user.GetEmail().GetValue(),
                Phone = user.GetPhone().GetValue(),
                UserType = user.GetUserType().ToString(),
                Status = user.GetStatus(),
                Department = user.GetDepartament().GetValue(),
            };

            var bsonDocument = new BsonDocument
                {
                    { "_id", mongoUser.Id },
                    { "Name", mongoUser.Name },
                    { "Email", mongoUser.Email },
                    { "Phone", mongoUser.Phone },
                    { "UserType", mongoUser.UserType },
                    { "Status", mongoUser.Status },
                    { "Department", mongoUser.Department }
                };

            await _userCollection.InsertOneAsync(bsonDocument);

            var savedUser = new User(
                new UserId(mongoUser.Id),
                new UserName(mongoUser.Name),
                new UserEmail(mongoUser.Email),
                new UserPhone(mongoUser.Phone),
                Enum.Parse<UserType>(mongoUser.UserType),
                mongoUser.Status,
                new DeptoId(mongoUser.Department)
            );

            return Result<User>.Success(savedUser);
        }

        public async Task<List<User>> GetAll(int perPage, int page)
        {
            var userEntities = await _userCollection
                .Find(_ => true)
                .Skip(perPage * (page - 1))
                .Limit(perPage)
                .ToListAsync();

            var users = userEntities.Select(e => new User(
                new UserId(e.GetValue("_id").AsString),
                new UserName(e.GetValue("Name").AsString),
                new UserEmail(e.GetValue("Email").AsString),
                new UserPhone(e.GetValue("Phone").AsString),
                Enum.Parse<UserType>(e.GetValue("UserType").AsString),
                e.GetValue("Status").AsBoolean,
                new DeptoId(e.GetValue("Department").AsString)
            )).ToList();

            return users;
        }

        public async Task<Core.Utils.Optional.Optional<User>> GetById(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var userDocument = await _userCollection.Find(filter).FirstOrDefaultAsync();

            if (userDocument is null)
            {
                return Core.Utils.Optional.Optional<User>.Empty();
            }

            var user = new User(
                new UserId(userDocument.GetValue("_id").AsString),
                new UserName(userDocument.GetValue("Name").AsString),
                new UserEmail(userDocument.GetValue("Email").AsString),
                new UserPhone(userDocument.GetValue("Phone").AsString),
                Enum.Parse<UserType>(userDocument.GetValue("UserType").AsString),
                userDocument.GetValue("Status").AsBoolean,
                new DeptoId(userDocument.GetValue("Department").AsString)
            );

            return Core.Utils.Optional.Optional<User>.Of(user);
        }
    }
}
