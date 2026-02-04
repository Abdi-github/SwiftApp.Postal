using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Address.Application.DTOs;
using SwiftApp.Postal.Modules.Address.Domain.Entities;
using SwiftApp.Postal.Modules.Address.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;

namespace SwiftApp.Postal.Modules.Address.Application.Services;

public class SwissAddressService(
    ISwissAddressRepository addressRepository,
    ILogger<SwissAddressService> logger)
{
    public async Task<PagedResult<SwissAddressResponse>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        // logger.LogDebug("SwissAddressService.GetPaged page={Page}, size={Size}", page, size);
        var result = await addressRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(MapToResponse).ToList();
        return new PagedResult<SwissAddressResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<SwissAddressResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var address = await addressRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(SwissAddress), id);
        return MapToResponse(address);
    }

    public async Task<List<SwissAddressResponse>> SearchByZipCodeAsync(string zipCode, CancellationToken ct = default)
    {
        var addresses = await addressRepository.GetByZipCodeAsync(zipCode, ct);
        return addresses.Select(MapToResponse).ToList();
    }

    public async Task<List<SwissAddressResponse>> SearchByCantonAsync(string canton, CancellationToken ct = default)
    {
        var addresses = await addressRepository.GetByCantonAsync(canton, ct);
        return addresses.Select(MapToResponse).ToList();
    }

    public async Task<SwissAddressResponse> CreateAsync(SwissAddressRequest request, CancellationToken ct = default)
    {
        // logger.LogDebug("Create Swiss address request: zip={ZipCode}, city={City}, canton={Canton}", request.ZipCode, request.City, request.Canton);
        var address = new SwissAddress
        {
            Id = Guid.NewGuid(),
            ZipCode = request.ZipCode,
            City = request.City,
            Canton = request.Canton,
            Municipality = request.Municipality
        };

        await addressRepository.AddAsync(address, ct);
        // System.Diagnostics.Debug.WriteLine($"SwissAddress persisted: id={address.Id}, zip={address.ZipCode}");
        logger.LogInformation("Swiss address created: {ZipCode} {City}", address.ZipCode, address.City);

        return MapToResponse(address);
    }

    public async Task<SwissAddressResponse> UpdateAsync(Guid id, SwissAddressRequest request, CancellationToken ct = default)
    {
        var address = await addressRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(SwissAddress), id);

        address.ZipCode = request.ZipCode;
        address.City = request.City;
        address.Canton = request.Canton;
        address.Municipality = request.Municipality;

        await addressRepository.UpdateAsync(address, ct);
        logger.LogInformation("Swiss address updated: {Id} {ZipCode} {City}", id, address.ZipCode, address.City);

        return MapToResponse(address);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var address = await addressRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(SwissAddress), id);

        await addressRepository.SoftDeleteAsync(id, ct);
        logger.LogInformation("Swiss address soft-deleted: {Id}", id);
    }

    private static SwissAddressResponse MapToResponse(SwissAddress a) => new(
        a.Id, a.ZipCode, a.City, a.Canton, a.Municipality);
}
