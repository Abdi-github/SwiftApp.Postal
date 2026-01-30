using FluentValidation;
using SwiftApp.Postal.Modules.Branch.Application.DTOs;

namespace SwiftApp.Postal.Modules.Branch.Application.Validators;

public class BranchRequestValidator : AbstractValidator<BranchRequest>
{
    public BranchRequestValidator()
    {
        RuleFor(x => x.BranchCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Type).NotEmpty().Must(t => t is "PostOffice" or "SortingCenter" or "DistributionCenter" or "Agency")
            .WithMessage("Type must be PostOffice, SortingCenter, DistributionCenter, or Agency");
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Canton).MaximumLength(5);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null).MaximumLength(256);
        RuleFor(x => x.Translations).NotEmpty().WithMessage("At least one translation is required");
        RuleForEach(x => x.Translations).ChildRules(t =>
        {
            t.RuleFor(tr => tr.Locale).NotEmpty().MaximumLength(10);
            t.RuleFor(tr => tr.Name).NotEmpty().MaximumLength(200);
        });
    }
}
