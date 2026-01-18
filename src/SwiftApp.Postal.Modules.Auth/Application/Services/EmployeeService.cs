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

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IMediator mediator,
    ILogger<EmployeeService> logger)
{
    public async Task<PagedResult<EmployeeResponse>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var result = await employeeRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(MapToResponse).ToList();
        return new PagedResult<EmployeeResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<EmployeeResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Employee), id);
        return MapToResponse(employee);
    }

    public async Task<EmployeeResponse> CreateAsync(EmployeeRequest request, CancellationToken ct = default)
    {
        // logger.LogDebug("Create employee request: Role={Role}, AssignedBranch={AssignedBranchId}, HireDate={HireDate}", request.Role, request.AssignedBranchId, request.HireDate);
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeNumber = GenerateEmployeeNumber(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Role = Enum.Parse<EmployeeRole>(request.Role),
            Status = EmployeeStatus.Active,
            AssignedBranchId = request.AssignedBranchId,
            HireDate = request.HireDate,
            PreferredLocale = request.PreferredLocale
        };

        await employeeRepository.AddAsync(employee, ct);
        logger.LogInformation("Employee created: {EmployeeNumber} ({Email})", employee.EmployeeNumber, employee.Email);

        await mediator.Publish(new EmployeeCreatedEvent(employee.Id, employee.EmployeeNumber, employee.Email), ct);

        return MapToResponse(employee);
    }

    public async Task<EmployeeResponse> UpdateAsync(Guid id, EmployeeRequest request, CancellationToken ct = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Employee), id);

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.Phone = request.Phone;
        employee.Role = Enum.Parse<EmployeeRole>(request.Role);
        employee.AssignedBranchId = request.AssignedBranchId;
        employee.HireDate = request.HireDate;
        employee.PreferredLocale = request.PreferredLocale;

        await employeeRepository.UpdateAsync(employee, ct);
        logger.LogInformation("Employee updated: {EmployeeNumber}", employee.EmployeeNumber);

        return MapToResponse(employee);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await employeeRepository.SoftDeleteAsync(id, ct);
        logger.LogInformation("Employee soft-deleted: {Id}", id);
    }

    public async Task<EmployeeResponse> SyncKeycloakUserAsync(EmployeeSyncRequest request, CancellationToken ct = default)
    {
        var employee = await employeeRepository.GetByEmailAsync(request.Email, ct)
            ?? throw new EntityNotFoundException("Employee", request.Email);

        // System.Diagnostics.Debug.WriteLine($"Identity sync request for employee role={employee.Role}");
        employee.KeycloakUserId = request.KeycloakUserId;
        await employeeRepository.UpdateAsync(employee, ct);
        logger.LogInformation("Employee keycloak sync: {Email} -> {KeycloakUserId}", request.Email, request.KeycloakUserId);

        return MapToResponse(employee);
    }

    private static EmployeeResponse MapToResponse(Employee e) => new(
        e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.Email, e.Phone,
        e.Role.ToString(), e.Status.ToString(), e.AssignedBranchId,
        e.HireDate, e.PreferredLocale, e.CreatedAt);

    private static string GenerateEmployeeNumber() => $"EMP-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpperInvariant()}";
}
