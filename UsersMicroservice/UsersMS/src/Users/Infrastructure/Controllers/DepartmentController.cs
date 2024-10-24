using Microsoft.AspNetCore.Mvc;
using UsersMS.src.Users.Application.Commands.CreateDepto.Types;
using UsersMS.src.Users.Infrastructure.Dto;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.Core.Application.IdGenerator;
using FluentValidation;
using UsersMS.Core.Application.Logger;
using UsersMS.src.Users.Application.Commands.CreateDepto;

namespace UsersMS.src.Users.Infrastructure.Controllers
{

    [Route("api/depto")]
    [ApiController]
    public class DeptoController(IDeptoRepository deptoRepo, IdGenerator<string> idGenerator, IValidator<CreateDeptoCommand> validator, ILoggerContract logger)
    {
        private readonly IDeptoRepository _deptoRepo = deptoRepo;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateDeptoCommand> _validator = validator;
        private readonly ILoggerContract _logger = logger;

        [HttpPost]
        public async Task<CreateDeptoResponse> CreateDepartment([FromBody] CreateDeptoDto createDeptoDto)
        {
            try
            {
                var command = new CreateDeptoCommand(createDeptoDto.Name, createDeptoDto.Description);

                var result = _validator.Validate(command);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for CreateDeptoCommand:", string.Join(", ", errors));
                    return new CreateDeptoResponse(string.Empty, errors);
                }

                var handler = new CreateDeptoCommandHandler(_deptoRepo, _idGenerator);
                var executionResult = await handler.Execute(command);

                if (executionResult.IsSuccessful)
                {
                    _logger.Log("Department created successfully: {DeptoId}", executionResult.Unwrap().Id);
                    return executionResult.Unwrap();
                }
                else
                {
                    _logger.Error("Failed to create department: {ErrorMessage}", executionResult.ErrorMessage);
                    return new CreateDeptoResponse(string.Empty, [executionResult.ErrorMessage]);
                }
            }
            catch (Exception ex)
            { 
                _logger.Exception("An error occurred while creating the department.", ex.Message);
                return new CreateDeptoResponse(string.Empty, ["An internal server error occurred."]);
            }
        }  
    }
}
