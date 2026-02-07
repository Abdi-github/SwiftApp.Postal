namespace SwiftApp.Postal.Modules.Tracking.Domain.Enums;

public enum TrackingEventType
{
    Registered,
    PickedUp,
    ArrivedAtSorting,
    DepartedSorting,
    ArrivedAtBranch,
    OutForDelivery,
    Delivered,
    DeliveryFailed,
    Returned
}
