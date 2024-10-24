using MongoDB.Bson;
using MongoDB.Driver;
using UsersMS.Core.Infrastructure.Data;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain.Entities;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Infrastructure.Models;

namespace UsersMS.src.Users.Infrastructure.Repositories
{
    public class MongoDeptoRepository(MongoDbService mongoDbService) : IDeptoRepository
    {
        private readonly IMongoCollection<BsonDocument> _deptoCollection = mongoDbService.GetDepartmentCollection();

        public async Task<bool> ExistByName(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var result = await _deptoCollection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }

        public async Task<Result<Department>> Save(Department department)
        {
            var mongoDepto = new MongoDepartment
            {
                Id = department.GetId().GetValue(),
                Name = department.GetName().GetValue(),
                Description = department.GetDescription().GetValue(),
                Status = department.GetStatus()
            };
            Console.WriteLine(mongoDepto);

            var bsonDocument = new BsonDocument
                {
                    { "_id", mongoDepto.Id },
                    { "Name", mongoDepto.Name },
                    { "Description", mongoDepto.Description },
                    { "Status", mongoDepto.Status }
                };

            await _deptoCollection.InsertOneAsync(bsonDocument);

            var savedDepto = new Department(
                new DeptoId(mongoDepto.Id),
                new DeptoName(mongoDepto.Name),
                new DeptoDescription(mongoDepto.Description),
                mongoDepto.Status
            );

            return Result<Department>.Success(savedDepto);
        }

        public async Task<List<Department>> GetAll(int perPage, int page)
        {
            var deptoEntities = await _deptoCollection
                .Find(_ => true)
                .Skip(perPage * (page - 1))
                .Limit(perPage)
                .ToListAsync();

            var deptos = deptoEntities.Select(e => new Department(
                new DeptoId(e.GetValue("_id").AsString),
                new DeptoName(e.GetValue("Name").AsString),
                new DeptoDescription(e.GetValue("Description").AsString),
                e.GetValue("Status").AsBoolean
            )).ToList();

            return deptos;
        }

        public async Task<Core.Utils.Optional.Optional<Department>> GetById(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var deptoDocument = await _deptoCollection.Find(filter).FirstOrDefaultAsync();

            if (deptoDocument is null)
            {
                return Core.Utils.Optional.Optional<Department>.Empty();
            }

            var depto = new Department(
                new DeptoId(deptoDocument.GetValue("_id").AsString),
                new DeptoName(deptoDocument.GetValue("Name").AsString),
                new DeptoDescription(deptoDocument.GetValue("Description").AsString),
                deptoDocument.GetValue("Status").AsBoolean
            );

            return Core.Utils.Optional.Optional<Department>.Of(depto);
        }
    }
}
