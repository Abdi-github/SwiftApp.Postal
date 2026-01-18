using SwiftApp.Postal.Modules.Auth.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Auth.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public Guid? AssignedBranchId { get; set; }
    public DateOnly HireDate { get; set; }
    public string PreferredLocale { get; set; } = "de";
    public string? KeycloakUserId { get; set; }
}
