using OrganizationalStructure.Application.Integration.Iam;

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