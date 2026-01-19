using FluentValidation;
using SwiftApp.Postal.Modules.Delivery.Application.DTOs;

namespace SwiftApp.Postal.Modules.Delivery.Application.Validators;

public class DeliveryRouteRequestValidator : AbstractValidator<DeliveryRouteRequest>
{
    public DeliveryRouteRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Slots).NotEmpty().WithMessage("At least one delivery slot is required");
        RuleForEach(x => x.Slots).ChildRules(s =>
        {
            s.RuleFor(sl => sl.TrackingNumber).NotEmpty().MaximumLength(20);
            s.RuleFor(sl => sl.SequenceOrder).GreaterThan(0);
        });
    }
}

public class PickupRequestDtoValidator : AbstractValidator<PickupRequestDto>
{
    public PickupRequestDtoValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.PickupStreet).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PickupZipCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PickupCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PreferredDate).NotEmpty();
    }
}
