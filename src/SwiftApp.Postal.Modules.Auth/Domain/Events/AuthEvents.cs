using SwiftApp.Postal.SharedKernel.Events;

namespace SwiftApp.Postal.Modules.Auth.Domain.Events;

public record EmployeeCreatedEvent(Guid EmployeeId, string EmployeeNumber, string Email) : IDomainEvent;
public record CustomerCreatedEvent(Guid CustomerId, string CustomerNumber, string Email) : IDomainEvent;
