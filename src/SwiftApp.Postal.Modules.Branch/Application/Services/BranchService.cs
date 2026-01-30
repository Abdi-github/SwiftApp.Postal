using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Branch.Application.DTOs;
using SwiftApp.Postal.Modules.Branch.Domain.Entities;
using SwiftApp.Postal.Modules.Branch.Domain.Enums;
using SwiftApp.Postal.Modules.Branch.Domain.Events;
using SwiftApp.Postal.Modules.Branch.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;
using SwiftApp.Postal.SharedKernel.Services;

namespace SwiftApp.Postal.Modules.Branch.Application.Services;

public class BranchService(
    IBranchRepository branchRepository,
    IMediator mediator,
    ILogger<BranchService> logger)
{
    public async Task<PagedResult<BranchResponse>> GetPagedAsync(int page, int size, string locale, CancellationToken ct = default)
    {
        var result = await branchRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(b => MapToResponse(b, locale)).ToList();
        return new PagedResult<BranchResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<BranchResponse> GetByIdAsync(Guid id, string locale, CancellationToken ct = default)
    {
        var branch = await branchRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Branch), id);
        return MapToResponse(branch, locale);
    }

    public async Task<BranchResponse> CreateAsync(BranchRequest request, string locale, CancellationToken ct = default)
    {
        // logger.LogDebug("Creating branch {BranchCode} ({Type}) in {City}", request.BranchCode, request.Type, request.City);
        var branch = new Domain.Entities.Branch
        {
            Id = Guid.NewGuid(),
            BranchCode = request.BranchCode,
            // TODO: Add validation for unique branch codes
            Type = Enum.Parse<BranchType>(request.Type),
            Status = BranchStatus.Active,
            Street = request.Street,
            ZipCode = request.ZipCode,
            City = request.City,
            Canton = request.Canton,
            Phone = request.Phone,
            Email = request.Email,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Translations = request.Translations.Select(t => new BranchTranslation
            {
                Id = Guid.NewGuid(),
                Locale = t.Locale,
                Name = t.Name,
                Description = t.Description
            }).ToList()
        };

        await branchRepository.AddAsync(branch, ct);

        logger.LogInformation("Branch created: {BranchCode}", branch.BranchCode);

        await mediator.Publish(new BranchCreatedEvent(branch.Id, branch.BranchCode), ct);

        return MapToResponse(branch, locale);
    }

    public async Task<BranchResponse> UpdateAsync(Guid id, BranchRequest request, string locale, CancellationToken ct = default)
    {
        var branch = await branchRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Branch), id);

        branch.BranchCode = request.BranchCode;
        branch.Type = Enum.Parse<BranchType>(request.Type);
        branch.Street = request.Street;
        branch.ZipCode = request.ZipCode;
        branch.City = request.City;
        branch.Canton = request.Canton;
        branch.Phone = request.Phone;
        branch.Email = request.Email;
        branch.Latitude = request.Latitude;
        branch.Longitude = request.Longitude;

        // Update translations
        branch.Translations.Clear();
        // System.Diagnostics.Debug.WriteLine($"Updating translations for branch {branch.BranchCode}: incoming={request.Translations.Count}");
        foreach (var t in request.Translations)
        {
            branch.Translations.Add(new BranchTranslation
            {
                Id = Guid.NewGuid(),
                Locale = t.Locale,
                Name = t.Name,
                Description = t.Description,
                BranchId = branch.Id
            });
        }

        await branchRepository.UpdateAsync(branch, ct);
        logger.LogInformation("Branch updated: {BranchCode}", branch.BranchCode);

        return MapToResponse(branch, locale);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await branchRepository.SoftDeleteAsync(id, ct);
        logger.LogInformation("Branch soft-deleted: {Id}", id);
    }

    private static BranchResponse MapToResponse(Domain.Entities.Branch b, string locale) => new(
        b.Id, b.BranchCode, b.Type.ToString(), b.Status.ToString(),
        b.Street, b.ZipCode, b.City, b.Canton, b.Phone, b.Email,
        b.Latitude, b.Longitude,
        TranslationResolver.Resolve(b.Translations, locale, t => t.Name),
        TranslationResolver.Resolve(b.Translations, locale, t => t.Description),
        b.CreatedAt);
}
