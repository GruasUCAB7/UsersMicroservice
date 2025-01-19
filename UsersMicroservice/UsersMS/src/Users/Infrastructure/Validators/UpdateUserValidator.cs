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
                .NotNull().WithMessage("Department is required.")
                .When(x => !string.IsNullOrEmpty(x.Department));

            RuleFor(x => x.UserType)
                .NotNull().WithMessage("UserType is required.")
                .When(x => !string.IsNullOrEmpty(x.UserType));
        }
    }
}