namespace SwiftApp.Postal.Modules.Address.Application.DTOs;

public record SwissAddressRequest(
    string ZipCode,
    string City,
    string Canton,
    string? Municipality);

public record SwissAddressResponse(
    Guid Id,
    string ZipCode,
    string City,
    string Canton,
    string? Municipality);
