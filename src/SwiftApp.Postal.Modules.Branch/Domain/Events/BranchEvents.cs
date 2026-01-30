using SwiftApp.Postal.SharedKernel.Events;

namespace SwiftApp.Postal.Modules.Branch.Domain.Events;

public record BranchCreatedEvent(Guid BranchId, string BranchCode) : IDomainEvent;
