using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftApp.Postal.Modules.Delivery.Application.Services;
using SwiftApp.Postal.Modules.Delivery.Application.Validators;
using SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;
using SwiftApp.Postal.Modules.Delivery.Infrastructure;
using SwiftApp.Postal.Modules.Delivery.Infrastructure.Persistence.Repositories;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.Modules.Delivery;

public class DeliveryModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDeliveryRouteRepository, DeliveryRouteRepository>();
        services.AddScoped<IPickupRequestRepository, PickupRequestRepository>();
        services.AddScoped<IDeliveryModuleApi, DeliveryModuleApiFacade>();
        services.AddScoped<DeliveryRouteService>();
        services.AddScoped<PickupRequestService>();
        services.AddValidatorsFromAssemblyContaining<DeliveryRouteRequestValidator>();
    }
}
