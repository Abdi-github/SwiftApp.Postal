namespace SwiftApp.Postal.Modules.Branch.Application.DTOs;

public record BranchTranslationRequest(string Locale, string Name, string? Description);

public record BranchRequest(
    string BranchCode,
    string Type,
    string Street,
    string ZipCode,
    string City,
    string? Canton,
    string? Phone,
    string? Email,
    decimal? Latitude,
    decimal? Longitude,
    List<BranchTranslationRequest> Translations);

public record BranchResponse(
    Guid Id,
    string BranchCode,
    string Type,
    string Status,
    string Street,
    string ZipCode,
    string City,
    string? Canton,
    string? Phone,
    string? Email,
    decimal? Latitude,
    decimal? Longitude,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt);
