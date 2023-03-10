using System;
using FluentValidation;
using Topluluk.Services.AuthenticationAPI.Model.Dto;

namespace Topluluk.Services.AuthenticationAPI.Model.Validators
{
	public class CreateUserValidator : AbstractValidator<CreateUserDto>
	{
		public CreateUserValidator()
		{

            RuleFor(u => u.FirstName)
                .NotEmpty()
                .NotNull()
                    .WithMessage("First Name must be required")
                .MinimumLength(3)
                .MaximumLength(64)
                    .WithMessage("First Name must be between length min 3 and max 64 length");

            RuleFor(u => u.LastName)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Last Name must be required")
                .MinimumLength(3)
                .MaximumLength(64)
                    .WithMessage("Last Name must be between length min 3 and max 64 length");

            RuleFor(u => u.UserName)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Username must be required")
                .MinimumLength(3)
                .MaximumLength(64)
                    .WithMessage("Username must be between length min 3 and max 64 length");

            RuleFor(u => u.Password)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Password must be required")
                .MinimumLength(6)
                .MaximumLength(64)
                    .WithMessage("Password must be between length min 6 and max 64 length");

         
            RuleFor(u => u.Email)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Username must be required")
                .EmailAddress()
                    .WithMessage("Please enter a valid email address.");
        }
    }
}

