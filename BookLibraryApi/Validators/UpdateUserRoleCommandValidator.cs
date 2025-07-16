using BookLibraryApi.Features.Users.Commands;
using FluentValidation;

namespace BookLibraryApi.Validators
{
    public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleCommandValidator() 
        {
            RuleFor(x => x.dto.UserId)
                .GreaterThan(0)
                .NotEmpty();
            RuleFor(x => x.dto.Role)
                .NotEmpty()
                .MinimumLength(3)
                .MinimumLength(20)
                .Matches("^[a-zA-Z0-9]*$")
                .Must(role => new[] { "User", "Admin" }//
                .Contains(role)) // tylko role typu User, Admin, Moderator itd. były akceptowane
                .WithMessage("Rola musi być jedną z: User, Admin"); ;
        }
    }
}
