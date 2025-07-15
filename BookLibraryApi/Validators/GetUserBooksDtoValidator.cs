using BookLibraryApi.Features.Users.Queries;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class GetUserBooksDtoValidator : AbstractValidator<GetUserBooksQuery>
    {
        public GetUserBooksDtoValidator() 
        {
            RuleFor(x => x.dto.Author)
                .MaximumLength(100);
            RuleFor(x => x.dto.Title)
                .MaximumLength(100);
            RuleFor(x => x.dto.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1-100");
            RuleFor(x => x.dto.Page)
                .GreaterThan(0)
                .WithMessage("Page musi być większe niż 0");
        }
    }
}
