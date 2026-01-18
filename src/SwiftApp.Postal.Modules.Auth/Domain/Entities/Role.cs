using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Auth.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<Permission> Permissions { get; set; } = [];
}
