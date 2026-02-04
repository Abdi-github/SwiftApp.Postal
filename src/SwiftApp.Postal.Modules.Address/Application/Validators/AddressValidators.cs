using FluentValidation;
using SwiftApp.Postal.Modules.Address.Application.DTOs;

namespace SwiftApp.Postal.Modules.Address.Application.Validators;

public class SwissAddressRequestValidator : AbstractValidator<SwissAddressRequest>
{
    public SwissAddressRequestValidator()
    {
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Canton).NotEmpty().MaximumLength(5);
        RuleFor(x => x.Municipality).MaximumLength(100);
    }
}
