using FluentValidation;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Domain.ValueObjects;

namespace UsersMS.src.Users.Infrastructure.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("The name must not be less than 3 characters.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^\+58 \d{3}-\d{7}$").WithMessage("Phone must be in the format +58 xxx-xxxx.");

            RuleFor(x => x.UserType)
                .NotEmpty().WithMessage("UserType is required.")
                .IsEnumName(typeof(UserType), caseSensitive: false).WithMessage("UserType is not valid.");
        }
    }
}
