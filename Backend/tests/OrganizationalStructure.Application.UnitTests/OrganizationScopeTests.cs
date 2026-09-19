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

    /// <summary>
    /// فهرست سازمان‌های قابل نمایش باید نام/کد/والد/عمق را از خود سازمان تا برگ‌ها برگرداند.
    /// </summary>
    [Fact]
    public void ComputeVisibleOrganizations_ShouldCarryNamesCodeAndDepth()
    {
        var leaf = new IamOrganizationNode { Id = Guid.NewGuid(), Name = "گروه توسعه", Code = "GRAND" };
        var middle = new IamOrganizationNode
        {
            Id = Guid.NewGuid(),
            Name = "واحد فناوری",
            Code = "CHILD",
            Children = new List<IamOrganizationNode> { leaf }
        };
        var sibling = new IamOrganizationNode { Id = Guid.NewGuid(), Name = "واحد پشتیبانی", Code = "SIB" };
        var root = new IamOrganizationNode
        {
            Id = Guid.NewGuid(),
            Name = "شرکت آزمون",
            Code = "ROOT",
            Children = new List<IamOrganizationNode> { middle, sibling }
        };

        var visible = OrganizationScope.ComputeVisibleOrganizations(new[] { root }, middle.Id);

        visible.Should().HaveCount(2);
        visible[0].Id.Should().Be(middle.Id);
        visible[0].Name.Should().Be("واحد فناوری");
        visible[0].Code.Should().Be("CHILD");
        visible[0].ParentId.Should().BeNull();
        visible[0].Depth.Should().Be(0);
        visible[1].Id.Should().Be(leaf.Id);
        visible[1].Name.Should().Be("گروه توسعه");
        visible[1].ParentId.Should().Be(middle.Id);
        visible[1].Depth.Should().Be(1);
    }

    /// <summary>
    /// سازمان ناموجود در درخت نباید هیچ گزینه‌ای بدهد (fail-closed).
    /// </summary>
    [Fact]
    public void ComputeVisibleOrganizations_UnknownOrganization_ShouldBeEmpty()
    {
        var root = Node(Guid.NewGuid(), Node(Guid.NewGuid()));

        var visible = OrganizationScope.ComputeVisibleOrganizations(new[] { root }, Guid.NewGuid());

        visible.Should().BeEmpty();
    }
}