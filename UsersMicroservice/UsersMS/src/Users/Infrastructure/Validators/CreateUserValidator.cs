using FluentValidation;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;

namespace UsersMS.src.Users.Infrastructure.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
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
                .NotEmpty().WithMessage("UserType is required.");
            
            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required.")
                .MinimumLength(2).WithMessage("The department must not be less than 2 characters.")
                .MaximumLength(25).WithMessage("Department must not exceed 25 characters.");
        }
    }
}
