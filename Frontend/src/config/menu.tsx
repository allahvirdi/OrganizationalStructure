import type { SvgIconComponent } from "@mui/icons-material";
import DashboardIcon from "@mui/icons-material/Dashboard";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import WorkIcon from "@mui/icons-material/Work";
import PeopleIcon from "@mui/icons-material/People";
import AssignmentIcon from "@mui/icons-material/Assignment";
import VerifiedUserIcon from "@mui/icons-material/VerifiedUser";

/**
 * قلم منوی اصلی (مرتبط با روت‌ها + Permission لازم).
 * آیتم بدون permission برای هر کاربر احرازهشده دیده می‌شود.
 */
export interface MenuItem {
  id: string;
  label: string;
  path: string;
  icon: SvgIconComponent;
  permission?: string;
}

export const mainMenu: MenuItem[] = [
  { id: "dashboard", label: "داشبورد", path: "/dashboard", icon: DashboardIcon },
  {
    id: "org-chart",
    label: "چارت سازمانی",
    path: "/org-chart",
    icon: AccountTreeIcon,
    permission: "OrganizationStructure.Post.ViewHierarchy",
  },
  {
    id: "posts",
    label: "پست‌ها",
    path: "/posts",
    icon: WorkIcon,
    permission: "OrganizationStructure.Post.View",
  },
  {
    id: "employees",
    label: "پرسنل",
    path: "/employees",
    icon: PeopleIcon,
    permission: "OrganizationStructure.Employee.View",
  },
  {
    id: "responsibilities",
    label: "مسئولیت‌ها",
    path: "/responsibilities",
    icon: AssignmentIcon,
    permission: "OrganizationStructure.Responsibility.View",
  },
  {
    id: "authorities",
    label: "اختیارها",
    path: "/authorities",
    icon: VerifiedUserIcon,
    permission: "OrganizationStructure.Authority.View",
  },
];

/**
 * پالایش منو بر اساس دسترسی‌های کاربر.
 */
export function filterMenuByPermissions(
  permissions: readonly string[] | null | undefined,
): MenuItem[] {
  return mainMenu.filter(
    (item) =>
      item.permission === undefined || permissions?.includes(item.permission),
  );
}
