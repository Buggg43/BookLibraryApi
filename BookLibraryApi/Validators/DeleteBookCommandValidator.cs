using BookLibraryApi.Features.Users.Commands;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
    {
        public DeleteBookCommandValidator() 
        {
            RuleFor(x => x.BookId)
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(x => x.User)
                .NotEmpty();
        }
    }
}
