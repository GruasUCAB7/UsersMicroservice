using FluentValidation;
using UsersMS.src.Users.Application.Commands.UpdateUser.Types;

namespace UsersMS.src.Users.Infrastructure.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive must be true or false.")
                .When(x => x.IsActive.HasValue);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^\+\d{2,3} \d{3}-\d{7}$").WithMessage("Phone format is invalid.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required.")
                .MinimumLength(2).WithMessage("The department must not be less than 2 characters.")
                .MaximumLength(25).WithMessage("Department must not exceed 25 characters.");
        }
    }
}