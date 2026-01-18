using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftApp.Postal.Modules.Auth.Application.Services;
using SwiftApp.Postal.Modules.Auth.Application.Validators;
using SwiftApp.Postal.Modules.Auth.Domain.Interfaces;
using SwiftApp.Postal.Modules.Auth.Infrastructure;
using SwiftApp.Postal.Modules.Auth.Infrastructure.Persistence.Repositories;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.Modules.Auth;

public class AuthModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAuthModuleApi, AuthModuleApiFacade>();
        services.AddScoped<EmployeeService>();
        services.AddScoped<CustomerService>();
        services.AddValidatorsFromAssemblyContaining<EmployeeRequestValidator>();
    }
}
