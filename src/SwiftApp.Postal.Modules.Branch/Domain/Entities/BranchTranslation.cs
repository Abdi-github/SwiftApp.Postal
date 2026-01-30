using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Branch.Domain.Entities;

public class BranchTranslation : BaseTranslation
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
}
