using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftApp.Postal.Modules.Tracking.Application.Services;
using SwiftApp.Postal.Modules.Tracking.Application.Validators;
using SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;
using SwiftApp.Postal.Modules.Tracking.Infrastructure;
using SwiftApp.Postal.Modules.Tracking.Infrastructure.Persistence.Repositories;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.Modules.Tracking;

public class TrackingModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITrackingRecordRepository, TrackingRecordRepository>();
        services.AddScoped<ITrackingEventRepository, TrackingEventRepository>();
        services.AddScoped<ITrackingModuleApi, TrackingModuleApiFacade>();
        services.AddScoped<TrackingService>();
        services.AddValidatorsFromAssemblyContaining<TrackingEventRequestValidator>();
    }
}
