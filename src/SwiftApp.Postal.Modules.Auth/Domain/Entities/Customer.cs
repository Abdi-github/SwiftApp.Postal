using SwiftApp.Postal.Modules.Auth.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Auth.Domain.Entities;

public class Customer : BaseEntity
{
    public string CustomerNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public string PreferredLocale { get; set; } = "de";
    public string? KeycloakUserId { get; set; }
}
