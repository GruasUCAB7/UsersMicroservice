using MongoDB.Bson;
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
            var filter = Builders<BsonDocument>.Filter.Eq("email", email);
            var result = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }

        public async Task<List<User>> GetAll(GetAllUsersQuery data)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = data.IsActive?.ToLower() switch
            {
                "activo" => filterBuilder.Eq("isActive", true),
                "inactivo" => filterBuilder.Eq("isActive", false),
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
                    new UserName(e.GetValue("name").AsString),
                    new UserEmail(e.GetValue("email").AsString),
                    new UserPhone(e.GetValue("phone").AsString),
                    new UserType(e.GetValue("userType").AsString),
                    new DeptoName(e.GetValue("department").AsString)
                );

                user.SetIsActive(e.GetValue("isActive").AsBoolean);

                if (e.TryGetValue("isTemporaryPassword", out var isTemporaryPassword))
                {
                    user.SetTemporaryPassword(isTemporaryPassword.AsBoolean);
                }

                if (e.TryGetValue("passwordExpirationDate", out var passwordExpirationDate))
                {
                    user.SetPasswordExpirationDate(passwordExpirationDate.ToUniversalTime());
                }

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
                new UserName(userDocument.GetValue("name").AsString),
                new UserEmail(userDocument.GetValue("email").AsString),
                new UserPhone(userDocument.GetValue("phone").AsString),
                new UserType(userDocument.GetValue("userType").AsString),
                new DeptoName(userDocument.GetValue("department").AsString)
            );

            user.SetIsActive(userDocument.GetValue("isActive").AsBoolean);

            if (userDocument.TryGetValue("isTemporaryPassword", out var isTemporaryPassword))
            {
                user.SetTemporaryPassword(isTemporaryPassword.AsBoolean);
            }

            if (userDocument.TryGetValue("passwordExpirationDate", out var passwordExpirationDate))
            {
                user.SetPasswordExpirationDate(passwordExpirationDate.ToUniversalTime());
            }

            return Core.Utils.Optional.Optional<User>.Of(user);
        }


        public async Task<Core.Utils.Optional.Optional<User>> GetByEmail(string email)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("email", email);
            var userDocument = await _userCollection.Find(filter).FirstOrDefaultAsync();

            if (userDocument is null)
            {
                return Core.Utils.Optional.Optional<User>.Empty();
            }

            var user = User.CreateUser(
                new UserId(userDocument.GetValue("_id").AsString),
                new UserName(userDocument.GetValue("name").AsString),
                new UserEmail(userDocument.GetValue("email").AsString),
                new UserPhone(userDocument.GetValue("phone").AsString),
                new UserType(userDocument.GetValue("userType").AsString),
                new DeptoName(userDocument.GetValue("department").AsString)
            );

            user.SetIsActive(userDocument.GetValue("isActive").AsBoolean);

            if (userDocument.TryGetValue("passwordHash", out var passwordHash))
            {
                user.SetPasswordHash(passwordHash.AsString);
            }

            if (userDocument.TryGetValue("isTemporaryPassword", out var isTemporaryPassword))
            {
                user.SetTemporaryPassword(isTemporaryPassword.AsBoolean);
            }

            if (userDocument.TryGetValue("passwordExpirationDate", out var passwordExpirationDate))
            {
                user.SetPasswordExpirationDate(passwordExpirationDate.ToUniversalTime());
            }

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
                Department = user.GetDepartment(),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                PasswordHash = user.GetPasswordHash(),
                IsTemporaryPassword = user.GetTemporaryPassword(),
                PasswordExpirationDate = user.GetPasswordExpirationDate()
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
                { "updatedDate", mongoUser.UpdatedDate },
                { "passwordHash", mongoUser.PasswordHash },
                { "isTemporaryPassword", mongoUser.IsTemporaryPassword },
                { "passwordExpirationDate", mongoUser.PasswordExpirationDate }
            };

            await _userCollection.InsertOneAsync(bsonDocument);

            var savedUser = User.CreateUser(
                new UserId(mongoUser.Id),
                new UserName(mongoUser.Name),
                new UserEmail(mongoUser.Email),
                new UserPhone(mongoUser.Phone),
                new UserType(mongoUser.UserType),
                new DeptoName(mongoUser.Department)
            );
            return Result<User>.Success(savedUser);
        }


        public async Task<Result<User>> Update(User user)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", user.GetId());
            var updateDefinitionBuilder = Builders<BsonDocument>.Update;
            var updateDefinitions = new List<UpdateDefinition<BsonDocument>>();

            updateDefinitions.Add(updateDefinitionBuilder.Set("isActive", user.GetIsActive()));

            if (!string.IsNullOrEmpty(user.GetPhone()))
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("phone", user.GetPhone()));
            }

            if (!string.IsNullOrEmpty(user.GetDepartment()))
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("department", user.GetDepartment()));
            }

            if (!string.IsNullOrEmpty(user.GetUserType()))
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("userType", user.GetUserType()));
            }

            if (!string.IsNullOrEmpty(user.GetPasswordHash()))
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("passwordHash", user.GetPasswordHash()));
            }

            updateDefinitions.Add(updateDefinitionBuilder.Set("isTemporaryPassword", user.GetTemporaryPassword()));

            if (user.GetPasswordExpirationDate() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("passwordExpirationDate", user.GetPasswordExpirationDate()));
            }

            updateDefinitions.Add(updateDefinitionBuilder.Set("updatedDate", DateTime.UtcNow));

            var update = updateDefinitionBuilder.Combine(updateDefinitions);

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return Result<User>.Failure(new Exception("No se pudo actualizar el usuario."));
            }

            return Result<User>.Success(user);
        }


    }
}
