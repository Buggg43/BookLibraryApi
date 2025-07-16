using BookLibraryApi.Features.Users.Queries;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
    {
        public GetBookByIdQueryValidator() 
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0)
                .WithMessage("Must be number");
            RuleFor(x => x.User)
                .NotEmpty();
        }
    }
}
