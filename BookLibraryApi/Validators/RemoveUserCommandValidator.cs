using BookLibraryApi.Features.Users.Commands;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class RemoveUserCommandValidator : AbstractValidator<RemoveUserCommand>
    {
        public RemoveUserCommandValidator() 
        {
            RuleFor(x => x.id)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
