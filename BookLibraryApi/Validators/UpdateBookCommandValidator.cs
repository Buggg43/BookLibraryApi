using BookLibraryApi.Features.Users.Commands;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator() 
        {
            RuleFor(x => x.User)
                .NotEmpty();
            RuleFor(x => x.BookId)
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(x => x.dto.Author)
                .Matches("^[a-zA-Z]*$");
            RuleFor(x => x.dto.Title)
                .Matches("^[a-zA-Z0-9]*$");
        }
    }
}
