using FluentAssertions;
using NetArchTest.Rules;

namespace QuotationAccelerator.Architecture.Tests;

public class LayerDependencyTests
{
    [Fact]
    public void Domain_ShouldNotReference_Infrastructure()
    {
        var result = Types.InAssembly(typeof(Catalog.Domain.ProjectMetadata).Assembly)
            .ShouldNot()
            .HaveDependencyOn("QuotationAccelerator.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Catalog_ShouldNotReference_Desktop()
    {
        var result = Types.InAssembly(typeof(Catalog.Domain.ProjectMetadata).Assembly)
            .ShouldNot()
            .HaveDependencyOn("QuotationAccelerator.Desktop")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Inbox_ShouldNotReference_Infrastructure()
    {
        var result = Types.InAssembly(typeof(Inbox.Domain.InboxMessage).Assembly)
            .ShouldNot()
            .HaveDependencyOn("QuotationAccelerator.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
