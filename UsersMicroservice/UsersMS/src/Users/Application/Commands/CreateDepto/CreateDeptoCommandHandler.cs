using UsersMS.Core.Application.IdGenerator;
using UsersMS.Core.Application.Services;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Commands.CreateDepto.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Application.Exceptions;
using UsersMS.src.Users.Domain.Entities;

namespace UsersMS.src.Users.Application.Commands.CreateDepto
{
    public class CreateDeptoCommandHandler(
        IDeptoRepository deptoRepository,
        IdGenerator<string> idGenerator
    ) : IService<CreateDeptoCommand, CreateDeptoResponse>
    {
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IDeptoRepository _deptoRepository = deptoRepository;

        public async Task<Result<CreateDeptoResponse>> Execute(CreateDeptoCommand command)
        {
            Console.WriteLine("command");
            Console.WriteLine(command);
            var isDeptoExist = await _deptoRepository.ExistByName(command.Name);
            if (isDeptoExist)
            {
                return Result<CreateDeptoResponse>.Failure(new DeptoAlreadyExistException(command.Name));
            }

            var id = _idGenerator.Generate();
            var idDepto = new DeptoId(id);
            var nameDepto = new DeptoName(command.Name);
            var descriptionDepto = new DeptoDescription(command.Description);
            var status = true;

            Console.WriteLine(idDepto);
            Console.WriteLine(nameDepto);
            Console.WriteLine(descriptionDepto);
            Console.WriteLine(status);

            var depto = new Department(
                idDepto,
                nameDepto,
                descriptionDepto,
                status
            );

            Console.WriteLine("departament");
            Console.WriteLine(depto);
            await _deptoRepository.Save(depto);

            return Result<CreateDeptoResponse>.Success(new CreateDeptoResponse(id));
        }
    }
}
