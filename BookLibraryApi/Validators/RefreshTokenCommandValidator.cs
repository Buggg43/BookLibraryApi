using BookLibraryApi.Models.Dtos;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenCommandValidator() 
        {
            RuleFor(x => x.token)
                .NotEmpty()
                .MinimumLength(40)
                .MaximumLength(256);
        }
    }
}
