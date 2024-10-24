using FluentValidation;

namespace UsersMS.src.Users.Application.Commands.CreateDepto.Types 
{
    public class CreateDeptoCommandValidator : AbstractValidator<CreateDeptoCommand>
    {
        public CreateDeptoCommandValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(5).WithMessage("The description must not be less than 3 characters.")
                .MaximumLength(100).WithMessage("Description must be less than 100 characters.");
        }
    }
}
