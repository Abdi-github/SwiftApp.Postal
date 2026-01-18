namespace SwiftApp.Postal.Modules.Auth.Application.DTOs;

public record EmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Role,
    Guid? AssignedBranchId,
    DateOnly HireDate,
    string PreferredLocale = "de");

public record EmployeeResponse(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Role,
    string Status,
    Guid? AssignedBranchId,
    DateOnly HireDate,
    string PreferredLocale,
    DateTimeOffset CreatedAt);

public record CustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string PreferredLocale = "de");

public record CustomerResponse(
    Guid Id,
    string CustomerNumber,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Status,
    string PreferredLocale,
    DateTimeOffset CreatedAt);

public record EmployeeSyncRequest(string KeycloakUserId, string Email);

public record CustomerSyncRequest(string KeycloakUserId, string Email);
