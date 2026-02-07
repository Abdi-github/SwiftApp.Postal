using FluentValidation;
using SwiftApp.Postal.Modules.Tracking.Application.DTOs;

namespace SwiftApp.Postal.Modules.Tracking.Application.Validators;

public class TrackingEventRequestValidator : AbstractValidator<TrackingEventRequest>
{
    public TrackingEventRequestValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.EventType).NotEmpty();
        RuleFor(x => x.Location).MaximumLength(200);
    }
}
