using FluentValidation;
using SwiftApp.Postal.Modules.Parcel.Application.DTOs;

namespace SwiftApp.Postal.Modules.Parcel.Application.Validators;

public class ParcelRequestValidator : AbstractValidator<ParcelRequest>
{
    public ParcelRequestValidator()
    {
        RuleFor(x => x.Type).NotEmpty().Must(t => t is "Standard" or "Priority" or "Express" or "Registered" or "Insured")
            .WithMessage("Type must be Standard, Priority, Express, Registered, or Insured");
        RuleFor(x => x.WeightKg).GreaterThan(0).When(x => x.WeightKg.HasValue);
        RuleFor(x => x.SenderName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SenderStreet).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SenderZipCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.SenderCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.RecipientName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RecipientStreet).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RecipientZipCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.RecipientCity).NotEmpty().MaximumLength(100);
    }
}
