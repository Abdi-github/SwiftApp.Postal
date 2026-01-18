using FluentValidation;
using SwiftApp.Postal.Modules.Auth.Application.DTOs;

namespace SwiftApp.Postal.Modules.Auth.Application.Validators;

public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Role).NotEmpty().Must(r => r is "Admin" or "BranchManager" or "Employee")
            .WithMessage("Role must be Admin, BranchManager, or Employee");
        RuleFor(x => x.PreferredLocale).NotEmpty().MaximumLength(10);
    }
}

public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
{
    public CustomerRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.PreferredLocale).NotEmpty().MaximumLength(10);
    }
}


