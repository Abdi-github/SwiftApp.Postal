using SwiftApp.Postal.Modules.Branch.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Branch.Domain.Entities;

public class Branch : BaseEntity
{
    public string BranchCode { get; set; } = string.Empty;
    public BranchType Type { get; set; }
    public BranchStatus Status { get; set; } = BranchStatus.Active;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Canton { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public ICollection<BranchTranslation> Translations { get; set; } = [];
}
