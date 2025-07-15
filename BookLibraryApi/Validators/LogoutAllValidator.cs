using BookLibraryApi.Features.Users.Commands;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class LogoutAllValidator : AbstractValidator<LogoutAllCommand>
    {
        public LogoutAllValidator() 
        {
            RuleFor(x => x.user)
                .NotEmpty();
        }

    }
}
