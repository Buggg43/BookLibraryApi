using BookLibraryApi.Models.Dtos;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserCommandValidator() 
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9]*$")
                .WithMessage("Username can only contain letters and numbers within 5 - 20 chars");
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(20)
                .Matches(@"^\S+$")
                .WithMessage("Password should be in 5-20 char range without spaces");
        }
    }
}
