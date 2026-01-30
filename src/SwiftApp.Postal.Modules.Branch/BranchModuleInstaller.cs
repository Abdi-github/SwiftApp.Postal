using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftApp.Postal.Modules.Branch.Application.Services;
using SwiftApp.Postal.Modules.Branch.Application.Validators;
using SwiftApp.Postal.Modules.Branch.Domain.Interfaces;
using SwiftApp.Postal.Modules.Branch.Infrastructure;
using SwiftApp.Postal.Modules.Branch.Infrastructure.Persistence.Repositories;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.Modules.Branch;

public class BranchModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IBranchModuleApi, BranchModuleApiFacade>();
        services.AddScoped<BranchService>();
        services.AddValidatorsFromAssemblyContaining<BranchRequestValidator>();
    }
}
