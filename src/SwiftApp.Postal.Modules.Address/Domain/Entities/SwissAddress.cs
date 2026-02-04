using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Address.Domain.Entities;

public class SwissAddress : BaseEntity
{
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Canton { get; set; } = string.Empty;
    public string? Municipality { get; set; }
}
