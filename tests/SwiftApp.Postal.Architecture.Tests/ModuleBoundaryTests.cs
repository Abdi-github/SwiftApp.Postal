using Xunit;
using NetArchTest.Rules;
using FluentAssertions;
using SwiftApp.Postal.Modules.Auth;
using SwiftApp.Postal.Modules.Branch;
using SwiftApp.Postal.Modules.Address;
using SwiftApp.Postal.Modules.Parcel;
using SwiftApp.Postal.Modules.Delivery;
using SwiftApp.Postal.Modules.Tracking;
using SwiftApp.Postal.Modules.Notification;

namespace SwiftApp.Postal.Architecture.Tests;

public class ModuleBoundaryTests
{
    private static readonly string[] AllModuleNamespaces =
    [
        "SwiftApp.Postal.Modules.Auth",
        "SwiftApp.Postal.Modules.Branch",
        "SwiftApp.Postal.Modules.Address",
        "SwiftApp.Postal.Modules.Parcel",
        "SwiftApp.Postal.Modules.Delivery",
        "SwiftApp.Postal.Modules.Tracking",
        "SwiftApp.Postal.Modules.Notification",
    ];

    [Fact]
    public void AuthModule_ShouldNotReference_OtherModules()
    {
        AssertModuleHasNoCrossModuleDependencies(
            typeof(AuthModuleInstaller).Assembly,
            "SwiftApp.Postal.Modules.Auth");
    }

    [Fact]
    public void BranchModule_ShouldNotReference_OtherModules()
    {
        AssertModuleHasNoCrossModuleDependencies(
            typeof(BranchModuleInstaller).Assembly,
            "SwiftApp.Postal.Modules.Branch");
    }

    [Fact]
    public void AddressModule_ShouldNotReference_OtherModules()
    {
        AssertModuleHasNoCrossModuleDependencies(
            typeof(AddressModuleInstaller).Assembly,
            "SwiftApp.Postal.Modules.Address");
    }

    [Fact]
    public void ParcelModule_ShouldNotReference_OtherModules()
    {
        AssertModuleHasNoCrossModuleDependencies(
            typeof(ParcelModuleInstaller).Assembly,
            "SwiftApp.Postal.Modules.Parcel");
    }

    [Fact]
    public void DeliveryModule_ShouldNotReference_OtherModules()
    {
        AssertModuleHasNoCrossModuleDependencies(
            typeof(DeliveryModuleInstaller).Assembly,
            "SwiftApp.Postal.Modules.Delivery");
    }

    [Fact]
    public void TrackingModule_ShouldNotReference_OtherModules()
    {
        AssertModuleHasNoCrossModuleDependencies(
            typeof(TrackingModuleInstaller).Assembly,
            "SwiftApp.Postal.Modules.Tracking");
    }

    [Fact]
    public void NotificationModule_ShouldNotReference_OtherModules()
    {
        AssertModuleHasNoCrossModuleDependencies(
            typeof(NotificationModuleInstaller).Assembly,
            "SwiftApp.Postal.Modules.Notification");
    }

    private static void AssertModuleHasNoCrossModuleDependencies(
        System.Reflection.Assembly assembly,
        string selfNamespace)
    {
        var forbidden = AllModuleNamespaces
            .Where(ns => ns != selfNamespace)
            .ToArray();

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"{selfNamespace} must not reference other modules. " +
                     $"Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
