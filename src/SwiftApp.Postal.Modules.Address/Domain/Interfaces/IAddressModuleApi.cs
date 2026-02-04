namespace SwiftApp.Postal.Modules.Address.Domain.Interfaces;

public interface IAddressModuleApi
{
    Task<bool> IsValidZipCodeAsync(string zipCode, CancellationToken ct = default);
    Task<string?> GetCantonByZipCodeAsync(string zipCode, CancellationToken ct = default);
}
