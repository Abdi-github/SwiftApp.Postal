using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftApp.Postal.Modules.Address.Application.Services;
using SwiftApp.Postal.Modules.Address.Application.Validators;
using SwiftApp.Postal.Modules.Address.Domain.Interfaces;
using SwiftApp.Postal.Modules.Address.Infrastructure;
using SwiftApp.Postal.Modules.Address.Infrastructure.Persistence.Repositories;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.Modules.Address;

public class AddressModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISwissAddressRepository, SwissAddressRepository>();
        services.AddScoped<IAddressModuleApi, AddressModuleApiFacade>();
        services.AddScoped<SwissAddressService>();
        services.AddValidatorsFromAssemblyContaining<SwissAddressRequestValidator>();
    }
}
