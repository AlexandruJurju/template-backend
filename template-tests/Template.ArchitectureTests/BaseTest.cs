using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using Mono.Cecil;
using Template.API;
using Template.Application;
using Template.Domain;
using Template.Infrastructure;
using Assembly = System.Reflection.Assembly;

namespace Template.ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(DomainAssemblyMarker).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ApplicationAssemblyMarker).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(InfrastructureAssemblyMarker).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(PresentationAssemblyMarker).Assembly;
}

public abstract class ArchUnitBaseTest : BaseTest
{
    protected static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(DomainAssembly, ApplicationAssembly, InfrastructureAssembly, PresentationAssembly)
        .Build();

    protected static readonly IObjectProvider<IType> DomainLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(DomainAssembly).As("Domain Layer");

    protected static readonly IObjectProvider<IType> ApplicationLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(ApplicationAssembly).As("Application Layer");

    protected static readonly IObjectProvider<IType> InfrastructureLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(InfrastructureAssembly).As("Infrastructure Layer");

    protected static readonly IObjectProvider<IType> PresentationLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(PresentationAssembly).As("Presentation Layer");
}
