using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftApp.Postal.Modules.Parcel.Application.Services;
using SwiftApp.Postal.Modules.Parcel.Application.Validators;
using SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;
using SwiftApp.Postal.Modules.Parcel.Infrastructure;
using SwiftApp.Postal.Modules.Parcel.Infrastructure.Persistence.Repositories;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.Modules.Parcel;

public class ParcelModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IParcelRepository, ParcelRepository>();
        services.AddScoped<IParcelModuleApi, ParcelModuleApiFacade>();
        services.AddScoped<ParcelService>();
        services.AddValidatorsFromAssemblyContaining<ParcelRequestValidator>();
    }
}
