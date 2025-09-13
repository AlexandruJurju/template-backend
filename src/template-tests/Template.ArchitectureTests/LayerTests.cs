using ArchUnitNET.NUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Template.ArchitectureTests;

[TestFixture]
public class LayerTests : ArchUnitBaseTest
{
    [Test]
    public void DomainLayer_Should_NotHaveDependencyOnOtherLayers()
    {
        Types()
            .That().Are(DomainLayer)
            .Should().NotDependOnAny(ApplicationLayer)
            .AndShould().NotDependOnAny(InfrastructureLayer)
            .AndShould().NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }

    [Test]
    public void ApplicationLayer_Should_NotDependOnInfrastructureLayerOrPresentationLayer()
    {
        Types()
            .That().Are(ApplicationLayer)
            .Should().NotDependOnAny(InfrastructureLayer)
            .AndShould().NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }
}
