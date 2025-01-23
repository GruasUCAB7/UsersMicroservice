using FluentValidation;
using UsersMS.src.Users.Application.Commands.AddNotificationToken.Types;

namespace UsersMS.src.Users.Infrastructure.Validators
{
    public class AddNotificationTokenValidator : AbstractValidator<AddNotificationTokenCommand>
    {
        public AddNotificationTokenValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");
        }

    }
}
