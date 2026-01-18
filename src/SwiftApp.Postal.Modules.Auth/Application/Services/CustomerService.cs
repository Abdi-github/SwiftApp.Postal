using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Auth.Application.DTOs;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.Modules.Auth.Domain.Enums;
using SwiftApp.Postal.Modules.Auth.Domain.Events;
using SwiftApp.Postal.Modules.Auth.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;

namespace SwiftApp.Postal.Modules.Auth.Application.Services;

public class CustomerService(
    ICustomerRepository customerRepository,
    IMediator mediator,
    ILogger<CustomerService> logger)
{
    public async Task<PagedResult<CustomerResponse>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var result = await customerRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(MapToResponse).ToList();
        return new PagedResult<CustomerResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<CustomerResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await customerRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Customer), id);
        return MapToResponse(customer);
    }

    public async Task<CustomerResponse> CreateAsync(CustomerRequest request, CancellationToken ct = default)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CustomerNumber = GenerateCustomerNumber(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Status = CustomerStatus.Active,
            PreferredLocale = request.PreferredLocale
        };

        // logger.LogDebug("Creating customer: Email={Email}, PreferredLocale={PreferredLocale}", request.Email, request.PreferredLocale);
        await customerRepository.AddAsync(customer, ct);

        logger.LogInformation("Customer created: {CustomerNumber} ({Email})", customer.CustomerNumber, customer.Email);

        await mediator.Publish(new CustomerCreatedEvent(customer.Id, customer.CustomerNumber, customer.Email), ct);

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(Guid id, CustomerRequest request, CancellationToken ct = default)
    {
        var customer = await customerRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Customer), id);

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.PreferredLocale = request.PreferredLocale;

        await customerRepository.UpdateAsync(customer, ct);
        logger.LogInformation("Customer updated: {CustomerNumber}", customer.CustomerNumber);

        return MapToResponse(customer);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await customerRepository.SoftDeleteAsync(id, ct);
        logger.LogInformation("Customer soft-deleted: {Id}", id);
    }

    public async Task<CustomerResponse> SyncKeycloakUserAsync(CustomerSyncRequest request, CancellationToken ct = default)
    {
        var customer = await customerRepository.GetByEmailAsync(request.Email, ct)
            ?? throw new EntityNotFoundException("Customer", request.Email);

        // System.Diagnostics.Debug.WriteLine($"Identity sync request: hasExistingExternalId={customer.KeycloakUserId is not null}");
        customer.KeycloakUserId = request.KeycloakUserId;
        await customerRepository.UpdateAsync(customer, ct);
        logger.LogInformation("Customer keycloak sync: {Email} -> {KeycloakUserId}", request.Email, request.KeycloakUserId);

        return MapToResponse(customer);
    }

    private static CustomerResponse MapToResponse(Customer c) => new(
        c.Id, c.CustomerNumber, c.FirstName, c.LastName, c.Email, c.Phone,
        c.Status.ToString(), c.PreferredLocale, c.CreatedAt);

    private static string GenerateCustomerNumber() => $"CUS-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpperInvariant()}";
}
