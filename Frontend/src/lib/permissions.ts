/** بررسی دسترسی نمایشی؛ کنترل قطعی مجوز و محدوده سازمان در سرور انجام می‌شود. */
export function hasPermission(
  user: { roles?: readonly string[]; permissions?: readonly string[] | null } | null | undefined,
  permission: string,
): boolean {
  if (!user) return false;
  return user.roles?.includes("SystemAdmin") === true ||
    (user.roles?.includes("OrganizationStructureAdmin") === true &&
      (permission === "OrganizationStructure.Post.Create" || permission === "OrganizationStructure.Post.View")) ||
    user.permissions?.includes(permission) === true;
}
