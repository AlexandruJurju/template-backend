using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.NUnit;
using Shouldly;
using Template.Common.SharedKernel.Domain;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Template.ArchitectureTests;

[TestFixture]
public class DomainTests : ArchUnitBaseTest
{
    [Test]
    public void DomainEvents_Should_BePublic()
    {
        Types()
            .That().ResideInAssembly(DomainAssembly)
            .And().ImplementInterface(typeof(IDomainEvent))
            .Should().BePublic()
            .Check(Architecture);
    }

    [Test]
    public void DomainEvents_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That().ImplementInterface(typeof(IDomainEvent))
            .Should().BeSealed()
            .Check(Architecture);
    }

    [Test]
    public void DomainEvents_Should_HaveNameEndingWith_DomainEvent()
    {
        Types()
            .That().ResideInAssembly(DomainAssembly)
            .And().ImplementInterface(typeof(IDomainEvent))
            .Should().HaveNameEndingWith("DomainEvent")
            .Check(Architecture);
    }

    [Test]
    public void Entities_Should_HavePrivateParameterlessConstructor()
    {
        IEnumerable<Class> entities = Classes().That().AreAssignableTo(typeof(Entity)).GetObjects(Architecture);

        var failingClasses = new List<Class>();
        foreach (Class entity in entities)
        {
            IEnumerable<MethodMember> constructors = entity.GetConstructors();

            if (!constructors.Any(c => c.Visibility == Visibility.Private && !c.Parameters.Any()))
            {
                failingClasses.Add(entity);
            }
        }

        failingClasses.ShouldBeEmpty();
    }
}
