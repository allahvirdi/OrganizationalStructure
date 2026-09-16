using FluentAssertions;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Integration.Iam;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های محاسبه محدوده سازمانی.
/// </summary>
public sealed class OrganizationScopeTests
{
    private static IamOrganizationNode Node(Guid id, params IamOrganizationNode[] children) =>
        new() { Id = id, Children = children.ToList() };

    /// <summary>
    /// Scope باید خود سازمان و تمام زیرمجموعه‌ها را شامل شود، نه والد و همسایه را.
    /// </summary>
    [Fact]
    public void ComputeScope_ShouldIncludeSelfAndDescendantsOnly()
    {
        var leaf = Node(Guid.NewGuid());
        var middle = Node(Guid.NewGuid(), leaf);
        var sibling = Node(Guid.NewGuid());
        var root = Node(Guid.NewGuid(), middle, sibling);

        var scope = OrganizationScope.ComputeScope(new[] { root }, middle.Id);

        scope.Should().Contain(middle.Id);
        scope.Should().Contain(leaf.Id);
        scope.Should().NotContain(root.Id);
        scope.Should().NotContain(sibling.Id);
    }

    /// <summary>
    /// سازمان ناموجود در درخت باید Scope خالی بدهد (fail-closed).
    /// </summary>
    [Fact]
    public void ComputeScope_UnknownOrganization_ShouldBeEmpty()
    {
        var root = Node(Guid.NewGuid(), Node(Guid.NewGuid()));

        var scope = OrganizationScope.ComputeScope(new[] { root }, Guid.NewGuid());

        scope.Should().BeEmpty();
    }
}