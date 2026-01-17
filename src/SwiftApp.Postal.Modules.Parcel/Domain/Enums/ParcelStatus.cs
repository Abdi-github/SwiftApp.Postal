namespace SwiftApp.Postal.Modules.Parcel.Domain.Enums;

public enum ParcelStatus
{
    Created,
    LabelGenerated,
    PickedUp,
    InTransit,
    OutForDelivery,
    Delivered,
    Returned,
    Cancelled
}
