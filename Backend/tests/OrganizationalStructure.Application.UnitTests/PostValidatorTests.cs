using FluentAssertions;
using OrganizationalStructure.Application.Posts.GetPostById;
using OrganizationalStructure.Application.Posts.GetPostChildren;
using OrganizationalStructure.Application.Posts.GetPostSubtree;
using OrganizationalStructure.Application.Posts.MovePost;
using OrganizationalStructure.Application.Posts.SearchPosts;
using OrganizationalStructure.Application.Posts.SetPostStatus;
using OrganizationalStructure.Application.Posts.UpdatePost;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های اعتبارسنج Queryها و دستورهای ساده پست.
/// </summary>
public sealed class PostValidatorTests
{
    /// <summary>
    /// شناسه خالی در Queryها باید نامعتبر باشد.
    /// </summary>
    [Fact]
    public void Queries_EmptyId_ShouldBeInvalid()
    {
        new GetPostByIdQueryValidator().Validate(new GetPostByIdQuery(Guid.Empty)).IsValid.Should().BeFalse();
        new GetPostChildrenQueryValidator().Validate(new GetPostChildrenQuery(Guid.Empty)).IsValid.Should().BeFalse();
        new MovePostCommandValidator().Validate(new MovePostCommand(Guid.Empty, null)).IsValid.Should().BeFalse();
        new SetPostStatusCommandValidator().Validate(new SetPostStatusCommand(Guid.Empty, true)).IsValid.Should().BeFalse();
        new UpdatePostCommandValidator().Validate(new UpdatePostCommand(Guid.Empty, "C", "T", null)).IsValid.Should().BeFalse();
    }

    /// <summary>
    /// عمق خارج از بازه در زیرشاخه باید نامعتبر باشد.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Subtree_OutOfRangeDepth_ShouldBeInvalid(int? depth)
    {
        new GetPostSubtreeQueryValidator().Validate(new GetPostSubtreeQuery(Guid.NewGuid(), depth)).IsValid.Should().BeFalse();
    }

    /// <summary>
    /// صفحه‌بندی نامعتبر در جستجو باید رد شود.
    /// </summary>
    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public void Search_InvalidPaging_ShouldBeInvalid(int page, int pageSize)
    {
        new SearchPostsQueryValidator().Validate(new SearchPostsQuery(null, null, null, page, pageSize)).IsValid.Should().BeFalse();
    }
}