using ArchUnitNET.NUnit;
using FluentValidation;
using Template.Common.SharedKernel.Application.CQRS.Commands;
using Template.Common.SharedKernel.Application.CQRS.Queries;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Template.ArchitectureTests;

[TestFixture]
public class ApplicationTests : ArchUnitBaseTest
{
    [Test]
    public void CommandHandler_Should_HaveNameEndingWith_CommandHandler()
    {
        Classes()
            .That().ResideInAssembly(ApplicationAssembly)
            .And().ImplementInterface(typeof(ICommandHandler<,>))
            .Should().HaveNameEndingWith("CommandHandler")
            .Check(Architecture);
    }

    [Test]
    public void CommandHandler_Should_NotBePublic()
    {
        Classes()
            .That().ResideInAssembly(ApplicationAssembly)
            .And().ImplementInterface(typeof(ICommandHandler<,>))
            .Should().NotBePublic()
            .Check(Architecture);
    }

    [Test]
    public void CommandHandler_Should_BeSealed()
    {
        Classes()
            .That().ResideInAssembly(ApplicationAssembly)
            .And().ImplementInterface(typeof(ICommandHandler<,>))
            .Should().BeSealed()
            .Check(Architecture);
    }

    [Test]
    public void QueryHandler_Should_HaveNameEndingWith_CommandHandler()
    {
        Classes()
            .That().ResideInAssembly(ApplicationAssembly)
            .And().ImplementInterface(typeof(IQueryHandler<,>))
            .Should().HaveNameEndingWith("QueryHandler")
            .Check(Architecture);
    }

    [Test]
    public void QueryHandler_Should_NotBePublic()
    {
        Classes()
            .That().ResideInAssembly(ApplicationAssembly)
            .And().ImplementInterface(typeof(IQueryHandler<,>))
            .Should().NotBePublic()
            .Check(Architecture);
    }

    [Test]
    public void Validator_Should_HaveNameEndingWith_Validator()
    {
        Classes()
            .That().ResideInAssembly(ApplicationAssembly)
            .And().AreAssignableTo(typeof(AbstractValidator<>))
            .Should().HaveNameEndingWith("Validator")
            .Check(Architecture);
    }

    [Test]
    public void Validator_Should_NotBePublic()
    {
        Classes()
            .That().ResideInAssembly(ApplicationAssembly)
            .And().AreAssignableTo(typeof(AbstractValidator<>))
            .Should().NotBePublic()
            .Check(Architecture);
    }
}
