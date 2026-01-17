using SwiftApp.Postal.Modules.Parcel.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Parcel.Domain.Entities;

public class Parcel : BaseEntity
{
    public string TrackingNumber { get; set; } = string.Empty;
    public ParcelStatus Status { get; set; } = ParcelStatus.Created;
    public ParcelType Type { get; set; } = ParcelType.Standard;
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal Price { get; set; }
    public Guid? SenderCustomerId { get; set; }
    public Guid? OriginBranchId { get; set; }

    // Sender address
    public string SenderName { get; set; } = string.Empty;
    public string SenderStreet { get; set; } = string.Empty;
    public string SenderZipCode { get; set; } = string.Empty;
    public string SenderCity { get; set; } = string.Empty;
    public string? SenderPhone { get; set; }

    // Recipient address
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientStreet { get; set; } = string.Empty;
    public string RecipientZipCode { get; set; } = string.Empty;
    public string RecipientCity { get; set; } = string.Empty;
    public string? RecipientPhone { get; set; }
}
