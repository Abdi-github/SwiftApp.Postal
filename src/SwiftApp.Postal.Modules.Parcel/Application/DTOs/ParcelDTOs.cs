namespace SwiftApp.Postal.Modules.Parcel.Application.DTOs;

public record ParcelStatusRequest(string Status);

public record ParcelRequest(
    string Type,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    Guid? SenderCustomerId,
    Guid? OriginBranchId,
    string SenderName,
    string SenderStreet,
    string SenderZipCode,
    string SenderCity,
    string? SenderPhone,
    string RecipientName,
    string RecipientStreet,
    string RecipientZipCode,
    string RecipientCity,
    string? RecipientPhone);

public record ParcelResponse(
    Guid Id,
    string TrackingNumber,
    string Status,
    string Type,
    decimal? WeightKg,
    decimal Price,
    string SenderName,
    string SenderZipCode,
    string SenderCity,
    string RecipientName,
    string RecipientZipCode,
    string RecipientCity,
    DateTimeOffset CreatedAt);
