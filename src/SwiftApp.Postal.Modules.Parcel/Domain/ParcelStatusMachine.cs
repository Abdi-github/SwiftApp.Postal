using SwiftApp.Postal.Modules.Parcel.Domain.Enums;

namespace SwiftApp.Postal.Modules.Parcel.Domain;

public static class ParcelStatusMachine
{
    private static readonly Dictionary<ParcelStatus, ParcelStatus[]> AllowedTransitions = new()
    {
        [ParcelStatus.Created] = [ParcelStatus.LabelGenerated, ParcelStatus.Cancelled],
        [ParcelStatus.LabelGenerated] = [ParcelStatus.PickedUp, ParcelStatus.Cancelled],
        [ParcelStatus.PickedUp] = [ParcelStatus.InTransit],
        [ParcelStatus.InTransit] = [ParcelStatus.OutForDelivery, ParcelStatus.Returned],
        [ParcelStatus.OutForDelivery] = [ParcelStatus.Delivered, ParcelStatus.Returned],
        [ParcelStatus.Delivered] = [],
        [ParcelStatus.Returned] = [],
        [ParcelStatus.Cancelled] = [],
    };

    public static bool CanTransition(ParcelStatus from, ParcelStatus to)
        => AllowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);

    public static ParcelStatus[] GetAllowedTransitions(ParcelStatus from)
        => AllowedTransitions.TryGetValue(from, out var targets) ? targets : [];
}
