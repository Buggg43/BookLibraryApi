using BookLibraryApi.Models.Dtos;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class LogoutCommandValidator : AbstractValidator<LogOutRequestDto>
    {
        public LogoutCommandValidator() 
        {
            RuleFor(x => x.refreshToken)
                .NotEmpty()
                .MinimumLength(20)
                .MaximumLength(256);
        }
    }
}
