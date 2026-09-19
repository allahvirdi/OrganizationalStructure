using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Authorization;

/// <summary>
/// محاسبه محدوده سازمانی قابل مشاهده کاربر (خود سازمان + زیرمجموعه‌ها).
/// </summary>
/// <remarks>
/// تابع خالص و بدون وابستگی خارجی؛ ورودی آن درخت IAM است.
/// </remarks>
public static class OrganizationScope
{
    /// <summary>
    /// محاسبه مجموعه شناسه‌های سازمان‌های داخل Scope.
    /// </summary>
    /// <param name="roots">ریشه‌های درخت سازمان IAM</param>
    /// <param name="userOrganizationId">شناسه سازمان کاربر</param>
    /// <returns>خود سازمان + تمام زیرمجموعه‌ها (خالی در صورت نبود سازمان در درخت)</returns>
    public static IReadOnlySet<Guid> ComputeScope(
        IReadOnlyList<IamOrganizationNode> roots,
        Guid userOrganizationId)
    {
        var result = new HashSet<Guid>();

        var target = FindNode(roots, userOrganizationId);
        if (target is null)
        {
            return result;
        }

        Collect(target, result);
        return result;
    }

    /// <summary>
    /// محاسبه فهرست واحدهای سازمانی داخل Scope همراه با نام، کد، والد و عمق.
    /// </summary>
    /// <param name="roots">ریشه‌های درخت سازمان IAM</param>
    /// <param name="userOrganizationId">شناسه سازمان کاربر</param>
    /// <returns>
    /// سازمان خود کاربر (عمق ۰) و تمام زیرمجموعه‌ها به ترتیب پیمایش عمق‌اول؛
    /// فهرست خالی در صورت نبود سازمان کاربر در درخت (fail-closed).
    /// </returns>
    public static IReadOnlyList<OrganizationReference> ComputeVisibleOrganizations(
        IReadOnlyList<IamOrganizationNode> roots,
        Guid userOrganizationId)
    {
        var target = FindNode(roots, userOrganizationId);
        if (target is null)
        {
            return Array.Empty<OrganizationReference>();
        }

        var result = new List<OrganizationReference>();
        CollectVisible(target, parentId: null, depth: 0, result);
        return result;
    }

    private static void CollectVisible(
        IamOrganizationNode node,
        Guid? parentId,
        int depth,
        List<OrganizationReference> result)
    {
        result.Add(new OrganizationReference(node.Id, node.Name, node.Code, parentId, depth));

        foreach (var child in node.Children)
        {
            CollectVisible(child, node.Id, depth + 1, result);
        }
    }

    private static IamOrganizationNode? FindNode(
        IEnumerable<IamOrganizationNode> nodes,
        Guid id)
    {
        foreach (var node in nodes)
        {
            if (node.Id == id)
            {
                return node;
            }

            var found = FindNode(node.Children, id);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private static void Collect(IamOrganizationNode node, HashSet<Guid> result)
    {
        result.Add(node.Id);

        foreach (var child in node.Children)
        {
            Collect(child, result);
        }
    }
}