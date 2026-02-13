using Xunit;
using NetArchTest.Rules;
using FluentAssertions;
using SwiftApp.Postal.SharedKernel.Interfaces;
using SwiftApp.Postal.Modules.Auth;
using SwiftApp.Postal.Modules.Branch;
using SwiftApp.Postal.Modules.Address;
using SwiftApp.Postal.Modules.Parcel;
using SwiftApp.Postal.Modules.Delivery;
using SwiftApp.Postal.Modules.Tracking;
using SwiftApp.Postal.Modules.Notification;

namespace SwiftApp.Postal.Architecture.Tests;

public class NamingConventionTests
{
    [Theory]
    [InlineData(typeof(AuthModuleInstaller))]
    [InlineData(typeof(BranchModuleInstaller))]
    [InlineData(typeof(AddressModuleInstaller))]
    [InlineData(typeof(ParcelModuleInstaller))]
    [InlineData(typeof(DeliveryModuleInstaller))]
    [InlineData(typeof(TrackingModuleInstaller))]
    [InlineData(typeof(NotificationModuleInstaller))]
    public void ModuleInstallers_ShouldImplement_IModuleInstaller(Type installerType)
    {
        installerType.GetInterfaces()
            .Should().Contain(typeof(IModuleInstaller),
                because: $"{installerType.Name} must implement IModuleInstaller");
    }

    [Fact]
    public void Repositories_ShouldResideIn_InfrastructurePersistence()
    {
        var moduleAssemblies = new[]
        {
            typeof(AuthModuleInstaller).Assembly,
            typeof(BranchModuleInstaller).Assembly,
            typeof(AddressModuleInstaller).Assembly,
            typeof(ParcelModuleInstaller).Assembly,
            typeof(DeliveryModuleInstaller).Assembly,
            typeof(TrackingModuleInstaller).Assembly,
            typeof(NotificationModuleInstaller).Assembly,
        };

        foreach (var assembly in moduleAssemblies)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Repository")
                .And()
                .DoNotHaveNameStartingWith("I")
                .Should()
                .ResideInNamespaceContaining("Infrastructure.Persistence.Repositories")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: $"All repository implementations must reside in Infrastructure.Persistence.Repositories. " +
                         $"Failing: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }

    [Fact]
    public void Services_ShouldResideIn_Application()
    {
        var moduleAssemblies = new[]
        {
            typeof(AuthModuleInstaller).Assembly,
            typeof(BranchModuleInstaller).Assembly,
            typeof(AddressModuleInstaller).Assembly,
            typeof(ParcelModuleInstaller).Assembly,
            typeof(DeliveryModuleInstaller).Assembly,
            typeof(TrackingModuleInstaller).Assembly,
            typeof(NotificationModuleInstaller).Assembly,
        };

        foreach (var assembly in moduleAssemblies)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Service")
                .And()
                .DoNotHaveNameStartingWith("I")
                .And()
                .DoNotResideInNamespaceContaining("Infrastructure")
                .Should()
                .ResideInNamespaceContaining("Application.Services")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: $"All service implementations must reside in Application.Services. " +
                         $"Failing: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }

    [Fact]
    public void Controllers_ShouldResideIn_Controllers()
    {
        var moduleAssemblies = new[]
        {
            typeof(AuthModuleInstaller).Assembly,
            typeof(BranchModuleInstaller).Assembly,
            typeof(AddressModuleInstaller).Assembly,
            typeof(ParcelModuleInstaller).Assembly,
            typeof(DeliveryModuleInstaller).Assembly,
            typeof(TrackingModuleInstaller).Assembly,
            typeof(NotificationModuleInstaller).Assembly,
        };

        foreach (var assembly in moduleAssemblies)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Controller")
                .Should()
                .ResideInNamespaceContaining("Controllers")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: $"All controllers must reside in the Controllers namespace. " +
                         $"Failing: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }

    [Fact]
    public void Validators_ShouldResideIn_ApplicationValidators()
    {
        var moduleAssemblies = new[]
        {
            typeof(AuthModuleInstaller).Assembly,
            typeof(BranchModuleInstaller).Assembly,
            typeof(AddressModuleInstaller).Assembly,
            typeof(ParcelModuleInstaller).Assembly,
            typeof(DeliveryModuleInstaller).Assembly,
            typeof(TrackingModuleInstaller).Assembly,
            typeof(NotificationModuleInstaller).Assembly,
        };

        foreach (var assembly in moduleAssemblies)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Validator")
                .Should()
                .ResideInNamespaceContaining("Application.Validators")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: $"All validators must reside in Application.Validators. " +
                         $"Failing: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }
}
