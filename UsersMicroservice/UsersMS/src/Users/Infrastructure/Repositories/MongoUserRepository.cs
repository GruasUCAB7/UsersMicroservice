﻿using MongoDB.Bson;
using MongoDB.Driver;
using UsersMS.Core.Infrastructure.Data;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Queries.GetAllUsers.Types;
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

        public async Task<List<User>> GetAll(GetAllUsersQuery data)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = data.IsActive?.ToLower() switch
            {
                "active" => filterBuilder.Eq("isActive", true),
                "inactive" => filterBuilder.Eq("isActive", false),
                _ => filterBuilder.Empty
            };

            var userEntities = await _userCollection
                .Find(filter)
                .Skip(data.PerPage * (data.Page - 1))
                .Limit(data.PerPage)
                .ToListAsync();

            var users = userEntities.Select(e =>
            {
                var user = User.CreateUser(
                    new UserId(e.GetValue("_id").AsString),
                    new UserName(e.GetValue("Name").AsString),
                    new UserEmail(e.GetValue("Email").AsString),
                    new UserPhone(e.GetValue("Phone").AsString),
                    Enum.Parse<UserType>(e.GetValue("UserType").AsString),
                    new DeptoId(e.GetValue("Department").AsString)
                );

                user.SetIsActive(e.GetValue("isActive").AsBoolean);
                return user;

            }).ToList();

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

            var user = User.CreateUser(
                new UserId(userDocument.GetValue("_id").AsString),
                new UserName(userDocument.GetValue("Name").AsString),
                new UserEmail(userDocument.GetValue("Email").AsString),
                new UserPhone(userDocument.GetValue("Phone").AsString),
                Enum.Parse<UserType>(userDocument.GetValue("UserType").AsString),
                new DeptoId(userDocument.GetValue("Department").AsString)
            );

            user.SetIsActive(userDocument.GetValue("isActive").AsBoolean);

            return Core.Utils.Optional.Optional<User>.Of(user);
        }

        public async Task<Result<User>> Save(User user)
        {
            var mongoUser = new MongoUser
            {
                Id = user.GetId(),
                Name = user.GetName(),
                Email = user.GetEmail(),
                Phone = user.GetPhone(),
                UserType = user.GetUserType(),
                IsActive = user.GetIsActive(),
                Department = user.GetDepartament(),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };

            var bsonDocument = new BsonDocument
            {
                { "_id", mongoUser.Id },
                { "name", mongoUser.Name },
                { "email", mongoUser.Email },
                { "phone", mongoUser.Phone },
                { "userType", mongoUser.UserType },
                { "isActive", mongoUser.IsActive },
                { "department", mongoUser.Department },
                { "createdDate", mongoUser.CreatedDate },
                { "updatedDate", mongoUser.UpdatedDate }
            };

            await _userCollection.InsertOneAsync(bsonDocument);

            var savedUser = User.CreateUser(
                new UserId(mongoUser.Id),
                new UserName(mongoUser.Name),
                new UserEmail(mongoUser.Email),
                new UserPhone(mongoUser.Phone),
                Enum.Parse<UserType>(mongoUser.UserType),
                new DeptoId(mongoUser.Department)
            );
            return Result<User>.Success(savedUser);
        }

        public async Task<Result<User>> Update(User user)
        {

            var filter = Builders<BsonDocument>.Filter.Eq("_id", user.GetId());
            var update = Builders<BsonDocument>.Update
                .Set("isActive", user.GetIsActive())
                .Set("Phone", user.GetPhone());

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return Result<User>.Failure(new Exception("Failed to update user"));
            }

            return Result<User>.Success(user);
        }

    }
}
