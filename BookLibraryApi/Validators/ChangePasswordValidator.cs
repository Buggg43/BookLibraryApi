using BookLibraryApi.Models.Dtos;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(20)
                .Matches(@"^\S+$")
                .WithMessage("Password should be in 5-20 char range without spaces");
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(20)
                .Matches(@"^\S+$")
                .WithMessage("Password should be in 5-20 char range without spaces");

            //Możesz połączyć walidację długości i spacji w .Matches regexem: .Matches(@"^\S{5,20}$")
        }
    }
}
