using BookLibraryApi.Features.Users.Commands;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator() 
        {
            RuleFor(x => x.User)
                .NotEmpty();
            RuleFor(x => x.dto.Author)
                .NotEmpty()
                .Matches("^[a-zA-Z]*$");
            RuleFor(x => x.dto.Year)
                .NotEmpty();
            RuleFor(x => x.dto.Title)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]*$");
        }
    }
}
