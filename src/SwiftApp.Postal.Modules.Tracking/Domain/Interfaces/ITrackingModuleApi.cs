namespace SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;

public interface ITrackingModuleApi
{
    Task<string?> GetCurrentStatusAsync(string trackingNumber, CancellationToken ct = default);
}
