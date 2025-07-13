using BookLibraryApi.Models.Dtos;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9]*$")
                .WithMessage("Username może zawierać tylko litery i cyfry");
        }
    }
}
